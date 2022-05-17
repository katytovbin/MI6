using MI6.Entities;
using NetTopologySuite.Geometries;
using System.Threading.Tasks;

namespace MI6.GoogleAPI
{
    public interface IGoogleAPIHelper
    {
        Task<MissionEntity> UpdateMissionLocation(MissionEntity currentMission);
        Task<Point> CalculatePointByAddress(string address);
    }
}
