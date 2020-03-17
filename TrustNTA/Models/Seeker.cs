using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrustNTA.Models
{
    public class Seeker
    {
        public string userId { get; set; }
        public DateTime dateCreated { get; set; }
        public string trn { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public bool isGraduate { get; set; }
        public bool isVerified { get; set; }
        public string identifier { get; set; }
        public DateTime lastAccessed { get; set; }
        public string password { get; set; }
        public string employmentStatus { get; set; }
        public List<SeekerJobTypeInterested> jobTypesInterested { get; set; }
        public List<SeekerLocationsAvailability> locationsAvailable { get; set; }
        public string GetFormattedJobLocations() 
        {
            string result =  "";
            if (locationsAvailable != null) 
            {
                foreach (SeekerLocationsAvailability location in locationsAvailable) 
                {
                    result += location.locationName + " ,";
                }
                result.TrimEnd(' ').TrimEnd(',');
            }
            return "";
        }

        public string GetFormattedJobTypes()
        {
            string result = "";
            if (jobTypesInterested != null)
            {
                foreach (SeekerJobTypeInterested job in jobTypesInterested)
                {
                    result += job.jobName + " ,";
                }
                result.TrimEnd(' ').TrimEnd(',');
            }
            return "";
        }
    }
}