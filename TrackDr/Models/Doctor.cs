    using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TrackDr.Models
{
    public partial class Doctor
    {
        public Doctor()
        {
            ParentDoctor = new HashSet<ParentDoctor>();
        }

        public string DoctorId { get; set; }
        public string FirstName { get; set; }
        public virtual ICollection<ParentDoctor> ParentDoctor { get; set; }
    }

    public class Rootobject
    {
        public Meta meta { get; set; }
        public Datum[] data { get; set; }

    }

    public class SingleDoctor
    {
        public Meta meta { get; set; }
        public Datum data { get; set; }
        public string DistanceInMiles { get; set; }
        public string DistanceInTime { get; set; }
    }

    public class Meta
    {
        public int count { get; set; }
        public int skip { get; set; }
        public int limit { get; set; }
    }

    public class Datum
    {
        public Practice[] practices { get; set; }
        public Education[] educations { get; set; }
        public Profile profile { get; set; }
        public Insurance[] insurances { get; set; }
        public Specialty[] specialties { get; set; }
        //****Dr. UID
        public string uid { get; set; }
        public string npi { get; set; }
    }

    public class Profile
    {

        public string first_name { get; set; }
        public string last_name { get; set; }
        public string slug { get; set; }
        public string title { get; set; }
        public string image_url { get; set; }
        public string gender { get; set; }
        public Language[] languages { get; set; }
        public string bio { get; set; }
        public string middle_name { get; set; }
    }

    public class Language
    {
        public string name { get; set; }
        public string code { get; set; }
    }

    public class Practice
    {
        public string uid { get; set; }
        public string name { get; set; }
        public string[] insurance_uids { get; set; }
        public Visit_Address visit_address { get; set; }
        public Language1[] languages { get; set; }
    }

    public class Visit_Address
    {
        public string city { get; set; }
        public string state { get; set; }
        public string street { get; set; }
        public string zip { get; set; }
    }

    public class Language1
    {
        public string name { get; set; }
        public string code { get; set; }
    }

    public class Medium
    {
    }

    public class Versions
    {
    }

    public class Education
    {
        public string degree { get; set; }
    }

    public class Rating
    {
    }

    public partial class Insurance
    {
        public Insurance_Plan insurance_plan { get; set; }
        public Insurance_Provider insurance_provider { get; set; }
    }

    public class Insurance_Plan
    {
        public string uid { get; set; }
        public string name { get; set; }
        public string[] category { get; set; }
    }

    public class Insurance_Provider
    {
        public string uid { get; set; }
        public string name { get; set; }
    }

    public class Specialty
    {
        public string uid { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string category { get; set; }
    }

    public class License
    {
    }
}
