using MI6.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MI6.Service
{
    public interface IMissionManager
    {
        Task<bool> PopulateDatabase(IList<MissionEntity> dumpData);
        Task<bool> AddMission(MissionEntity mission);
        string FindMostIsolatedCountry();
        Task<MissionEntity> FindClosestMission(TargetLocationInput address);
    }
}
