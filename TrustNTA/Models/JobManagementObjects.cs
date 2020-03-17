using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrustNTA.Models
{
    public class JobManagementObjects
    {
        public List<Job> jobs { get; set; }
        public List<JobLocation> jobLocations { get; set; }
        public List<EmployerVacancy> employerVacancies { get; set; }
    }
}