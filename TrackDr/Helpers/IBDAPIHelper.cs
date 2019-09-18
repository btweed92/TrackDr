using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackDr.Models;

namespace TrackDr.Helpers
{
    public interface IBDAPIHelper
    {
        Task<SingleDoctor> GetDoctor(string doctorId);
        string GetAPIKey();
        Task<Rootobject> GetDoctorList(string userInput, string userState);
        Task<Rootobject> GetDoctorList();
        Task<Rootobject> GetDoctorsBaseOnInsurance(string userInsurance);
    }
}
