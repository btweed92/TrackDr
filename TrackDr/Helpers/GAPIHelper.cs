using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TrackDr.Models;

namespace TrackDr.Helpers
{
    public class GAPIHelper : IGAPIHelper
    {
        private readonly IConfiguration _configuration;
        private readonly IDatabaseHelper _dbHelper;
        public GAPIHelper(IConfiguration configuration, IDatabaseHelper dbHelper)
        {
            _configuration = configuration;
            _dbHelper = dbHelper;
        }
        public string GetAPIKey()
        {
            //Use this method to get the APIKey
            return _configuration.GetSection("AppConfiguration")["GAPIKeyValue"];
        }

        public string GooglefyUserAddress(string userName)
        {
            string returnString = "";
            AspNetUsers currentUser = _dbHelper.GetCurrentUser(userName);
            Parent currentParent = _dbHelper.GetCurrentParent(currentUser);

            returnString += GooglefyString(currentParent.HouseNumber);
            returnString += GooglefyString(currentParent.Street);

            if (currentParent.Street2 != null)
            { returnString += GooglefyString(currentParent.Street2); }

            returnString += GooglefyString(currentParent.City);
            returnString += (currentParent.State);

            return returnString;
        }
        public string GooglefyString(string toBeGooglefied)
        {
            string returnString = "";
            string[] words = toBeGooglefied.Split(' ');
            if (words.Length > 0)
            {
                foreach (string word in words)
                {
                    returnString += word + "+";
                }
            }
            else
            {
                return toBeGooglefied + "+";
            }
            return returnString;
        }
        public string GooglefyDoctorAddress(SingleDoctor doctor)
        {
            string returnString = "";
            returnString += GooglefyString(doctor.data.practices[0].visit_address.street);
            returnString += (doctor.data.practices[0].visit_address.state);
            return returnString;
        }
        public async Task<string> GetTravelInfo(string startAddress, string endAddress)
        {
            string apiKey = GetAPIKey();
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://maps.googleapis.com");
            var response = await client.GetAsync($"maps/api/distancematrix/json?units=imperial&origins={startAddress}&destinations={endAddress}&key={GetAPIKey()}");
            return await response.Content.ReadAsStringAsync();
        }
        public List<Row> GetTravelRoutes(string startAddress, string endAddress)
        {
            List<Row> travelRoutes = new List<Row>();
            TravelInfo travelInfo = JsonConvert.DeserializeObject<TravelInfo>(GetTravelInfo(startAddress, endAddress).Result);
            foreach (Row row in travelInfo.rows)
            {
                travelRoutes.Add(row);
            }
            return travelRoutes;
        }
        public string GetDistanceInMiles(List<Row> travelRoutes)
        {
            string distance = "Unable to calculate distance. Please check your profile address.";
            List<Element[]> elements = new List<Element[]>();
            foreach (Row row in travelRoutes)
            {
                elements.Add(row.elements);
            }

            foreach (Element[] elementArray in elements)
            {
                if (elementArray[0].distance != null)
                {
                    distance = elementArray[0].distance.text;
                }
            }
            return distance;
        }
        public string GetDistanceInTime(List<Row> travelRoutes)
        {
            string distance = "Unable to calculate time. Please check your profile address.";
            List<Element[]> elements = new List<Element[]>();
            foreach (Row row in travelRoutes)
            {
                elements.Add(row.elements);
            }
            if (elements.Count > 0)
            {
                foreach (Element[] elementArray in elements)
                {
                    if (elementArray[0].duration != null)
                    {
                        distance = elementArray[0].duration.text;
                    }
                }
            }
            return distance;
        }
    }
}
