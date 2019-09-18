using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrackDr.Helpers
{
    public interface IGAPIHelper
    {
        string GetAPIKey();
        string GooglefyUserAddress(string userName);
        string GooglefyString(string toBeGooglefied);
        Task<string> GetTravelInfo(string startAddress, string endAddress);
        List<Models.Row> GetTravelRoutes(string startAddress, string endAddress);
        string GetDistanceInMiles(List<Models.Row> travelRoutes);
        string GetDistanceInTime(List<Models.Row> travelRoutes);
        string GooglefyDoctorAddress(Models.SingleDoctor doctor);
    }
}
