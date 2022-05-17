using MI6.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace MI6.API.Dump
{
    public class DumpData
    {

        public List<MissionEntity> GetMissionDumpata()
        {
            var missionsDump = new List<MissionEntity>();
            try
            {
                missionsDump = JsonConvert.DeserializeObject<List<MissionEntity>>(File.ReadAllText(@"missions_dump.json"));
            }
            catch (System.Exception ex)
            {
                // add log
            }

            return missionsDump;
        }

    }
}
