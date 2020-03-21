using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrustNTA.Models
{
    public class SeekerDashboardData
    {
        public List<Company> companies { get; set; }
        public List<Job> jobTypes { get; set; }
        public List<JobLocation> jobLocations { get; set; }
        public List<EmployerVacancy> vacanciesLimited { get; set; }
        public List<EmployerVacancy> specificVacancies { get; set; }
    }
}