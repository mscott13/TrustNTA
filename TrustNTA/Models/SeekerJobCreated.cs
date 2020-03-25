using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrustNTA.Models
{
    public class SeekerJobCreated
    {
        public string userId { get; set; }
        public DateTime dateCreated { get; set; }
        public string formattedDate { get; set; }
        public string jobType { get; set; }
        public  string seekerJobId { get; set; }
        public bool isSeeking { get; set; }
        public string resume { get; set; }
        public string jobTypeName { get; set; }
        public string formattedLocations { get; set; }
        public string jobStatusDesc { get; set; }
        public List<JobLocation> locations { get; set; }

        public string GetJobStatusName()
        {
            if (isSeeking)
            {
                return Data_Access.Database.SEEKER_STATUS_SEEKING;
            }
            else 
            {
                return Data_Access.Database.SEEKER_STATUS_NOT_SEEKING;
            }
        }

        public string GetFormattedDate() 
        {
            return dateCreated.ToString("dd MMMM, yyyy");
        }

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