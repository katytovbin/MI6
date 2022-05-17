using MI6.Entities;
using MI6.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MI6.WebAPI.Controllers
{
    [Route("[action]")]
    [ApiController]
    public class MissionController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMissionManager _missionManager;        

        public MissionController(ILogger<MissionController> logger, IMissionManager missionManager)
        {
            _logger = logger;
            _missionManager = missionManager;            
        }


        [HttpPost]
        [ActionName("mission")]
        public async Task<string> AddMission(MissionEntity mission)
        {
            var iaAdded = false;
            try
            {
                iaAdded = await _missionManager.AddMission(mission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return "Something went wrong";
            }
            return iaAdded? "Added": "Not added";
        }


        [HttpGet]
        [ActionName("countries-by-isolation")]
        public string GetCountries()
        {
            var res = "No result";
            try
            {
                var country = _missionManager.FindMostIsolatedCountry();
                res = $"The most isolated country is { country }";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return "Something went wrong";
            }

            return res;
        }

        // {"lng":41.34227210235956,"lat":-100.7633214379561}

        [HttpPost]
        [ActionName("find-closest")]
        public async Task<string> FindClosestMission(TargetLocationInput targetLocation)
        {
            var message = string.Empty;
            try
            {
                var mission = await _missionManager.FindClosestMission(targetLocation);
                if (mission == null)
                    message = "Mission not found";
                else message = $"Mission is found. Agent: {mission.Agent}; country: {mission.Country}; address: {mission.Address} ";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                message = "Something went wrong";
            }

            return message;
        }

    }
}
