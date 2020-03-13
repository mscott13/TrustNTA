using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrustNTA.Models
{
    public class EmployerVacancy
    {
        public string userId { get; set; }
        public string employerVacancyId { get; set; }
        public string jobType { get; set; }
        public string jobTitle { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string vacancyStatus { get; set; }
        public DateTime dateCreated { get; set; }
        public List<JobLocation> locations { get; set; }
        public List<Seeker> interestedClients { get; set; }

        public string GetFormattedLocations() 
        {
            if (locations != null)
            {
                string result = "";
                foreach (JobLocation location in locations)
                {
                    result += location.locationName + ", ";
                }
                return result.TrimEnd(' ').TrimEnd(',');
            }
            else 
            {
                return "--";
            }
        }
    }
}