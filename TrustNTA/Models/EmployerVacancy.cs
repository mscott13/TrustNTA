using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrustNTA.Data_Access;

namespace TrustNTA.Models
{
    public class EmployerVacancy
    {
        public int index { get; set; }
        public string companyLogoUrl { get; set; }
        public string userId { get; set; }
        public string employerVacancyId { get; set; }

        private string _jobType;
        public string jobTypeId { get; set; }
        public string jobType 
        {
            get { return _jobType; }
            set 
            {
                html_jobType = Database.GetJobTypeName(value);
                _jobType = value;
            }
        }
        public string jobTitle { get; set; }

        private DateTime _startDate;
        public DateTime startDate 
        {
            get { return _startDate; }
            set 
            {
                html_startDate = value.ToString("dd MMMM, yyyy");
                _startDate = value;
            }
        }
        private DateTime _endDate;
        public DateTime endDate
        {
            get { return _endDate; }
            set
            {
                html_endDate = value.ToString("dd MMMM, yyyy");
                _endDate = value;
            }
        }

        private string _vacancyStatus;
        public string vacancyStatus 
        {
            get { return _vacancyStatus; }
            set 
            {
                if (value != null)
                {
                    if (value == Database.VACANCY_STATUS_OPEN)
                    {
                        html_vacancyStatus = "<i style='color: green;' class='fas fa-circle'></i>" + Database.VACANCY_STATUS_OPEN;
                    }
                    else if (value == Database.VACANCY_STATUS_FILLED)
                    {
                        html_vacancyStatus = "<i style='color: orange;' class='fas fa-circle'></i>" + Database.VACANCY_STATUS_FILLED;
                    }
                    else if (value == Database.VACANCY_STATUS_CLOSED)
                    {
                        html_vacancyStatus = "<i style='color: red;' class='fas fa-circle'></i>" + Database.VACANCY_STATUS_CLOSED;
                    }
                    else
                    {
                        html_vacancyStatus = value;
                    }
                    _vacancyStatus = value;
                }
                else 
                {
                    _vacancyStatus = value;
                }
            }
        }

        private DateTime _dateCreated;
        public DateTime dateCreated
        {
            get { return _dateCreated; }
            set
            {
                html_dateCreated = value.ToString("dd MMMM, yyyy");
                _dateCreated = value;
            }
        }

        private List<JobLocation>_locations;
        public List<JobLocation> locations 
        {
            get { return _locations; }
            set 
            {
                _locations = value;
                html_locations = GetFormattedLocations();
            }
        }
        public List<Seeker> interestedClients { get; set; }

        public Employer employer { get; set; }
        public string html_jobType { get; set; }
        public string html_startDate { get; set; }
        public string html_endDate { get; set; }
        public string html_dateCreated { get; set; }
        public string html_locations { get; set; }
        public string html_vacancyStatus { get; set; }

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