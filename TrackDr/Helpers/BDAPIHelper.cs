using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TrackDr.Models;

namespace TrackDr.Helpers
{
    public class BDAPIHelper : IBDAPIHelper
    {

        private readonly IConfiguration _configuration;
        public BDAPIHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // this method returns a single doctor based on the doctor's UID
        public async Task<SingleDoctor> GetDoctor(string doctorId)
        {
            string apiKey = GetAPIKey();
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.betterdoctor.com");
            var response = await client.GetAsync($"/2016-03-01/doctors/{doctorId}?user_key={apiKey}");
            var result = await response.Content.ReadAsAsync<SingleDoctor>();

            return result;
        }

        // this method returns the API key from where it is hidden in Appsettings.JSON
        public string GetAPIKey()
        {
            return _configuration.GetSection("AppConfiguration")["BDAPIKeyValue"];
        }
        
        // this method returns a list of doctors based on what the user enters in the search field
        public async Task<Rootobject> GetDoctorList(string userInput, string userState)
        {
            string apiKey = GetAPIKey();
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.betterdoctor.com");
            var response = await client.GetAsync($"/2016-03-01/doctors?query={userInput}&specialty_uid=pediatrician&limit=100&location={userState}&user_key={apiKey}"); 
            return await response.Content.ReadAsAsync<Rootobject>();
        }

        //this method returns a list of doctors based on what the user nothing in the search field
        public async Task<Rootobject> GetDoctorList()
        {
            string apiKey = GetAPIKey();
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.betterdoctor.com");
            var response = await client.GetAsync($"/2016-03-01/doctors?query=specialty_uid=pediatrician&limit=100&user_key={apiKey}");
            return await response.Content.ReadAsAsync<Rootobject>();
        }

        // this method returns doctors based on which base and specialty insurace the user chooses
        public async Task<Rootobject> GetDoctorsBaseOnInsurance(string userInsurance) 
        {
            string apiKey = GetAPIKey();
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.betterdoctor.com");
            var response = await client.GetAsync($"/2016-03-01/doctors?query=pediatrician&specialty_uid=pediatrician&insurance_uid={userInsurance}&skip=0&limit=10&user_key={apiKey}"); //EDITED THIS 
            return await response.Content.ReadAsAsync<Rootobject>();
        }
    }
}
