using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrustNTA.Models
{
    public class JobLocs_JobType
    {
        public List<JobLocation> jobLocations { get; set; }
        public Job jobType { get; set; }
        public EmployerVacancy vacancy { get; set; }
    }
}