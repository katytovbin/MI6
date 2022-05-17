using MI6.Dal;
using MI6.Entities;
using MI6.GoogleAPI;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MI6.Service
{
    public class MissionManager : IMissionManager
    {
        private readonly IGoogleAPIHelper _googleHelper;
        private readonly DatabaseContext _dbContext;
        private readonly ILogger<MissionManager> _logger;


        public MissionManager(IGoogleAPIHelper googleHelper, DatabaseContext dbContext, ILogger<MissionManager> logger)
        {
            _googleHelper = googleHelper;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<bool> PopulateDatabase(IList<MissionEntity> missions)
        {
            try
            {
                if (!_dbContext.Missions.Any())
                {
                    foreach (var entity in missions)
                    {
                        var updatedEntity = await _googleHelper.UpdateMissionLocation(entity);
                        _dbContext.Missions.Add(updatedEntity);
                    }

                    _dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // add log
                return false;
            }

            return true;
        }

        public async Task<bool> AddMission(MissionEntity mission)
        {
            var res = false;
            try
            {
                mission = await _googleHelper.UpdateMissionLocation(mission);

                if (Validate(mission))
                {
                    _dbContext.Missions.Add(mission);
                    _dbContext.SaveChanges();
                    res = true;
                }
            }
            catch (Exception ex)
            {
                // add error log
            }
            return res;
        }

        public string FindMostIsolatedCountry()
        {
            string mostIsolatedCountry = null;
            try
            {
                var copyEntity = new List<MissionEntity>(_dbContext.Missions);

                var mostIsolatedCountryObj = _dbContext.Missions.Select(m => new MissionEntity { Agent = m.Agent, Country = m.Country })
                                                               .GroupBy(a => a.Agent, (k, v) => new { Agent = k, Countries = v.ToList() })
                                                               .Where(ag => ag.Countries.Count() == 1)
                                                               .SelectMany(c => c.Countries)
                                                               .GroupBy(a => a.Country)
                                                               .Select(aa => new { Country = aa.Key, Count = aa.Count() })
                                                               .OrderByDescending(o => o.Count)
                                                               .First();

                mostIsolatedCountry = mostIsolatedCountryObj?.Country;
            }
            catch (Exception ex)
            {
                // add log
            }
            return mostIsolatedCountry;
        }

        public async Task<MissionEntity> FindClosestMission(TargetLocationInput targetLocation)
        {
            MissionEntity closestMission = null;
            try
            {
                var location = await SetLocationCoordinatesByTypeOfInput(targetLocation);

                if (location == null)
                    return null;

                closestMission = _dbContext.Missions.OrderBy(c => c.Location.Distance(location)).FirstOrDefault();

            }
            catch (Exception ex)
            {
                // add log
            }
            return closestMission;
        }

        private async Task<Point> SetLocationCoordinatesByTypeOfInput(TargetLocationInput targetLocation)
        {
            var location = !string.IsNullOrEmpty(targetLocation.Address) ?
                await _googleHelper.CalculatePointByAddress(targetLocation.Address) :
                new Point(targetLocation.Lng, targetLocation.Lat) { SRID = 4326 };

            return location;
        }

        private bool Validate(MissionEntity mission)
        {
            if (mission == null
                || string.IsNullOrEmpty(mission.Agent)
                || string.IsNullOrEmpty(mission.Address)
                || string.IsNullOrEmpty(mission.Address)
                || mission.Date == DateTime.MinValue
                || mission.Location == null)

                return false;

            return true;
        }
    }
}
