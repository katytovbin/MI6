using MI6.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MI6.GoogleAPI
{
    public class GoogleAPIHelper : IGoogleAPIHelper
    {
        private readonly string _baseApiUrl;
        private readonly string _apiKey;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GoogleAPIHelper> _logger;


        public GoogleAPIHelper(IConfiguration configuration, ILogger<GoogleAPIHelper> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var gooleApiConfig = _configuration.GetSection("GoogleApi");
            _baseApiUrl = gooleApiConfig["GoogleMapsApiUrl"];
            _apiKey = gooleApiConfig["GoogleApiAppKey"];
        }

        public async Task<MissionEntity> UpdateMissionLocation(MissionEntity currentMission)
        {
            try
            {
                currentMission.Location = await CalculatePointByAddress(currentMission.Address);
            }
            catch (Exception ex)
            {
                // add log
            }

            return currentMission;
        }

        public async Task<Point> CalculatePointByAddress(string address)
        {

            var url = $"{_baseApiUrl}/geocode/json?new_forward_geocoder=true&address={address}&key={_apiKey}";

            var callResult = await CallGetAsync(url);
            if (callResult.Contains("zero_results", StringComparison.OrdinalIgnoreCase)) return null;

            dynamic json = JObject.Parse(callResult);
            var location = json.results.Count > 0 ?
                new Point(json.results[0].geometry.location.lng.Value, json.results[0].geometry.location.lat.Value) { SRID = 4326 }
                : null;

            return location;

        }

        private async Task<string> CallGetAsync(string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);

                return await response.Content.ReadAsStringAsync();

            }
        }
    }
}
