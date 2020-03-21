using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using TrustNTA.Models;
using TrustNTA.Utilities;

namespace TrustNTA.Data_Access
{
    public static class Database
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;

        //TABLE NAMES
        private const string TBL_COMPANIES = "Companies";
        private const string TBL_CREDENTIALS = "Credentials";
        private const string TBL_EMPLOYER = "Employer";
        private const string TBL_EMPLOYER_JOBCREATED = "Employer_JobCreated";
        private const string TBL_JOB_LOCATION_AVAILABILITY = "Job_LocationAvailability";
        private const string TBL_JOB_LOCATIONS = "Job_Locations";
        private const string TBL_JOB_POOL = "JobPool";
        private const string TBL_SEEKER = "Seeker";
        private const string TBL_SEEKER_JOBCREATED = "Seeker_JobCreated";
        private const string TBL_SEEKER_JOB_INTERESTED = "Seeker_JobInterested";
        private const string TBL_SEEKER_ACCOUNT_ACTIVATION = "SeekerAccountActivation";

        //MISC
        private const string ACTIVATION_STATUS_COMPLETED = "COMPLETED";
        private const string ACTIVATION_STATUS_INCOMPLETE = "INCOMPLETE";
        public const string VACANCY_STATUS_OPEN = "Open";
        public const string VACANCY_STATUS_FILLED = "Filled";
        public const string VACANCY_STATUS_CLOSED = "Closed";
        public const string EMPLOYER_ACCOUNT_INDIVIDUAL = "INDIVIDUAL";
        public const string EMPLOYER_ACCOUNT_COMPANY = "COMPANY";

        public static EmployerVacancy GetEmployerVacancy(string jobId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Select * from Employer_JobCreated where employerJobId=@jobId and visibility=1";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@jobId", jobId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            EmployerVacancy vacancy = new EmployerVacancy
                            {
                                userId = reader["userId"].ToString(),
                                employerVacancyId = reader["employerJobId"].ToString(),
                                jobType = reader["jobType"].ToString(),
                                jobTitle = reader["jobTitle"].ToString(),
                                startDate = Convert.ToDateTime(reader["startDate"]),
                                endDate = Convert.ToDateTime(reader["endDate"]),
                                vacancyStatus = reader["vacancyStatus"].ToString(),
                                dateCreated = Convert.ToDateTime(reader["dateCreated"]),
                                employer = GetEmployerViaUID(reader["userId"].ToString())
                            };
                            return vacancy;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public static void SetSeekerAvailableLocations(string userId, List<SeekerLocationsAvailability> locations)
        {
            if (locations != null)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string cmd = "Insert into Seeker_LocationAvailability (locationId, userId, locationName) values (@locationId, @userId)";

                    using (SqlCommand command1 = new SqlCommand("Delete from Seeker_LocationAvailability where userId=@userId", connection))
                    {
                        command1.Parameters.AddWithValue("@userId", userId);
                        command1.ExecuteNonQuery();
                    }

                    foreach (SeekerLocationsAvailability location in locations)
                    {
                        using (SqlCommand command2 = new SqlCommand(cmd, connection))
                        {
                            command2.Parameters.AddWithValue("@userId", userId);
                            command2.Parameters.AddWithValue("@locationId", location.locationId);
                            command2.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public static List<SeekerLocationsAvailability> GetSeekerLocationsAvailability(string userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "select seeker.locationId, seeker.userId, job.locationName from  Seeker_LocationAvailability seeker " +
                             "inner join Job_Locations job on job.locationId = seeker.locationId where seeker.userId = @userId";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    List<SeekerLocationsAvailability> locations = new List<SeekerLocationsAvailability>();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                SeekerLocationsAvailability location = new SeekerLocationsAvailability
                                {
                                    locationId = Convert.ToInt32(reader["locationId"]),
                                    locationName = reader["locationName"].ToString(),
                                    userId = reader["userId"].ToString()
                                };
                                locations.Add(location);
                            }
                            return locations;
                        }
                        else
                        {
                            return new List<SeekerLocationsAvailability>();
                        }
                    }
                }
            }
        }

        public static List<SeekerJobTypeInterested> GetSeekerJobTypeInterests(string userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "select types.jobId, types.userId, pool.title from Seeker_InterestedJobTypes types " +
                             "inner join JobPool pool on pool.jobId = types.jobId where types.userId = @userId";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    List<SeekerJobTypeInterested> jobsInterested = new List<SeekerJobTypeInterested>();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                SeekerJobTypeInterested jobInterested = new SeekerJobTypeInterested
                                {
                                    jobId = reader["jobId"].ToString(),
                                    jobName = reader["title"].ToString(),
                                    userId = reader["userId"].ToString()
                                };
                                jobsInterested.Add(jobInterested);
                            }
                            return jobsInterested;
                        }
                        else
                        {
                            return new List<SeekerJobTypeInterested>();
                        }
                    }
                }
            }
        }

        public static void SetSeekerJobTypeIntrests(List<SeekerJobTypeInterested> jobInterests, string userId)
        {
            if (jobInterests != null)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string cmd = "Insert into Seeker_InterestedJobTypes (jobId, userId) values (@jobId, @userId)";

                    using (SqlCommand command1 = new SqlCommand("Delete from Seeker_InterestedJobTypes where userId=@userId", connection))
                    {
                        command1.Parameters.AddWithValue("@userId", userId);
                        command1.ExecuteNonQuery();
                    }

                    foreach (SeekerJobTypeInterested interest in jobInterests)
                    {
                        using (SqlCommand command2 = new SqlCommand(cmd, connection))
                        {
                            command2.Parameters.AddWithValue("@userId", userId);
                            command2.Parameters.AddWithValue("@jobId", interest.jobId);
                            command2.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public static SeekerDashboardData GetSeekerDashboardData(string userId)
        {
            SeekerDashboardData dashboardData = new SeekerDashboardData
            {
                companies = GetCompanies(),
                jobLocations = GetJobLocations(),
                jobTypes = GetJobTypes(),
                vacanciesLimited = GetAllEmployerVacanciesRange(0, 8, "", "", "", "", "", DateTime.MinValue, DateTime.MinValue),
                specificVacancies = GetSpecificVacanciesForSeeker(userId)
            };
            return dashboardData;
        }

        public static List<EmployerVacancy> GetSpecificVacanciesForSeeker(string userId)
        {

            Seeker seeker = GetSeekerViaUID(userId);
            if (seeker != null)
            {
                List<EmployerVacancy> matchedVacancies = new List<EmployerVacancy>();
                List<EmployerVacancy> allVacancies = GetAllEmployerVacancies();

                foreach (EmployerVacancy vacancy in allVacancies)
                {
                    bool jobTypeSuitable = false;
                    if (seeker.jobTypesInterested != null)
                    {
                        foreach (SeekerJobTypeInterested data in seeker.jobTypesInterested)
                        {
                            if (data.jobId != null && vacancy.jobTypeId != null)
                            {
                                if (data.jobId == vacancy.jobTypeId)
                                {
                                    jobTypeSuitable = true;
                                }
                            }
                        }
                    }

                    bool locSuitable = false;
                    if (seeker.locationsAvailable != null)
                    {
                        foreach (SeekerLocationsAvailability data in seeker.locationsAvailable)
                        {
                            if (vacancy.locations != null)
                            {
                                foreach (JobLocation location in vacancy.locations)
                                {
                                    if (data.locationId == location.locationId)
                                    {
                                        locSuitable = true;
                                    }
                                }
                            }
                        }
                    }

                    if (jobTypeSuitable)
                    {
                        matchedVacancies.Add(vacancy);
                    }
                }
                return matchedVacancies;
            }
            else
            {
                return new List<EmployerVacancy>();
            }
        }

        public static List<EmployerVacancy> GetAllEmployerVacancies()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "select * from Employer_JobCreated where visibility=1 order by dateCreated desc";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<EmployerVacancy> vacancies = new List<EmployerVacancy>();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                EmployerVacancy vacancy = new EmployerVacancy
                                {
                                    userId = reader["userId"].ToString(),
                                    employerVacancyId = reader["employerJobId"].ToString(),
                                    jobType = GetJobTypeName(reader["jobType"].ToString()),
                                    jobTypeId = reader["jobType"].ToString(),
                                    jobTitle = reader["jobTitle"].ToString(),
                                    startDate = Convert.ToDateTime(reader["startDate"]),
                                    endDate = Convert.ToDateTime(reader["endDate"]),
                                    vacancyStatus = reader["vacancyStatus"].ToString(),
                                    locations = GetAvailLocations(reader["employerJobId"].ToString()),
                                    employer = GetEmployerViaUID(reader["userId"].ToString())
                                };
                                vacancies.Add(vacancy);
                            }
                            return vacancies;
                        }
                        else
                        {
                            return new List<EmployerVacancy>();
                        }
                    }
                }
            }
        }

        public static List<EmployerVacancy> GetAllEmployerVacanciesRange(int offset, int limit, string filter_Location, string filter_Company, string filter_JobType, string filter_JobTitle, string filter_VacancyStatus, DateTime filter_StartDate, DateTime filter_EndDate)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "select * from Employer_JobCreated where visibility=1 order by dateCreated desc offset " + offset + " rows fetch next " + limit + " rows only";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<EmployerVacancy> vacancies = new List<EmployerVacancy>();
                        List<EmployerVacancy> filteredVacancies = new List<EmployerVacancy>();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                EmployerVacancy vacancy = new EmployerVacancy
                                {
                                    userId = reader["userId"].ToString(),
                                    employerVacancyId = reader["employerJobId"].ToString(),
                                    jobType = GetJobTypeName(reader["jobType"].ToString()),
                                    jobTitle = reader["jobTitle"].ToString(),
                                    startDate = Convert.ToDateTime(reader["startDate"]),
                                    endDate = Convert.ToDateTime(reader["endDate"]),
                                    vacancyStatus = reader["vacancyStatus"].ToString(),
                                    locations = GetAvailLocations(reader["employerJobId"].ToString()),
                                    employer = GetEmployerViaUID(reader["userId"].ToString())
                                };
                                vacancies.Add(vacancy);
                            }

                            if (filter_Location != null && filter_Location != "") 
                            {
                                filteredVacancies = FilterVacancyByLocation(vacancies, filter_Location);
                            }
                            if (filter_Company != null && filter_Company != "") 
                            {
                                filteredVacancies = FilterVacancyByCompany(filteredVacancies, filter_Company);
                            }
                            if (filter_JobType != null && filter_JobType != "") 
                            {
                                filteredVacancies = FilterVacancyByJobType(filteredVacancies, filter_JobType);
                            }
                            if (filter_JobTitle != null && filter_JobTitle != "")
                            {
                                filteredVacancies = FilterVacancyByJobTitle(filteredVacancies, filter_JobTitle);
                            }
                            if (filter_VacancyStatus != null && filter_VacancyStatus != "")
                            {
                                filteredVacancies = FilterVacancyByVacancyStatus(filteredVacancies, filter_VacancyStatus);
                            }
                            if (filter_StartDate != DateTime.MinValue)
                            {
                                filteredVacancies = FilterVacancyByStartDate(filteredVacancies, filter_StartDate);
                            }
                            if (filter_EndDate != DateTime.MinValue)
                            {
                                filteredVacancies = FilterVacancyByEndDate(filteredVacancies, filter_EndDate);
                            }

                            return filteredVacancies;
                        }
                        else
                        {
                            return new List<EmployerVacancy>();
                        }
                    }
                }
            }
        }

        public static List<EmployerVacancy> FilterVacancyByVacancyStatus(List<EmployerVacancy> vacancies, string vacancyStatus)
        {
            List<EmployerVacancy> tempVacancy = new List<EmployerVacancy>();
            if (vacancies != null)
            {
                foreach (EmployerVacancy filterVacany in vacancies)
                {
                    if (filterVacany.vacancyStatus != null && vacancyStatus != null)
                    {
                        if (filterVacany.vacancyStatus == vacancyStatus)
                        {
                            tempVacancy.Add(filterVacany);
                        }
                    }
                }
                return vacancies;
            }
            else
            {
                return null;
            }
        }

        public static List<EmployerVacancy> FilterVacancyByEndDate(List<EmployerVacancy> vacancies, DateTime endDate)
        {
            List<EmployerVacancy> tempVacancy = new List<EmployerVacancy>();
            if (vacancies != null)
            {
                foreach (EmployerVacancy filterVacany in vacancies)
                {
                    if (endDate != DateTime.MinValue)
                    {
                        if (filterVacany.endDate.Date == endDate.Date)
                        {
                            tempVacancy.Add(filterVacany);
                        }
                    }
                }
                return tempVacancy;
            }
            else
            {
                return null;
            }
        }

        public static List<EmployerVacancy> FilterVacancyByStartDate(List<EmployerVacancy> vacancies, DateTime startDate)
        {
            List<EmployerVacancy> tempVacancy = new List<EmployerVacancy>();
            if (vacancies != null)
            {
                foreach (EmployerVacancy filterVacany in vacancies)
                {
                    if (startDate != DateTime.MinValue)
                    {
                        if (filterVacany.startDate.Date == startDate.Date)
                        {
                            tempVacancy.Add(filterVacany);
                        }
                    }
                }
                return tempVacancy;
            }
            else
            {
                return null;
            }
        }

        public static List<EmployerVacancy> FilterVacancyByJobTitle(List<EmployerVacancy> vacancies, string jobTitle)
        {
            List<EmployerVacancy> tempVacancy = new List<EmployerVacancy>();
            if (vacancies != null)
            {
                foreach (EmployerVacancy filterVacany in vacancies)
                {
                    if (filterVacany.jobTitle != null)
                    {
                        if (filterVacany.jobTitle == jobTitle)
                        {
                            tempVacancy.Add(filterVacany);
                        }
                    }
                }
                return tempVacancy;
            }
            else
            {
                return null;
            }
        }

        public static List<EmployerVacancy> FilterVacancyByJobType(List<EmployerVacancy> vacancies, string jobTypeName)
        {
            List<EmployerVacancy> tempVacancy = new List<EmployerVacancy>();
            if (vacancies != null)
            {
                foreach (EmployerVacancy filterVacany in vacancies)
                {
                    if (jobTypeName != null && jobTypeName != "")
                    {
                        if (filterVacany.jobType != null)
                        {
                            if (filterVacany.jobType == jobTypeName)
                            {
                                tempVacancy.Add(filterVacany);
                            }
                        }
                    }
                }
                return tempVacancy;
            }
            else
            {
                return null;
            }
        }

        public static List<EmployerVacancy> FilterVacancyByCompany(List<EmployerVacancy> vacancies, string companyName)
        {
            List<EmployerVacancy> tempVacancy = new List<EmployerVacancy>();
            if (vacancies != null)
            {
                foreach (EmployerVacancy filterVacancy in vacancies)
                {
                    if (companyName != null && companyName != "")
                    {
                        if (filterVacancy.employer != null)
                        {
                            if (filterVacancy.employer.companyName == companyName)
                            {
                                tempVacancy.Add(filterVacancy);
                            }
                        }
                    }
                }
                return tempVacancy;
            }
            else
            {
                return null;
            }
        }

        public static List<EmployerVacancy> FilterVacancyByLocation(List<EmployerVacancy> vacancies, string locationName)
        {
            List<EmployerVacancy> tempVacancy = new List<EmployerVacancy>();
            if (vacancies != null)
            {
                foreach (EmployerVacancy filterVacancy in vacancies)
                {
                    if (locationName != null && locationName != "")
                    {
                        if (filterVacancy.locations != null)
                        {
                            foreach (JobLocation location in filterVacancy.locations)
                            {
                                if (location.locationName == locationName)
                                {
                                    tempVacancy.Add(filterVacancy);
                                    break;
                                }
                            }
                        }
                    }
                }
                return tempVacancy;
            }
            else
            {
                return null;
            }
        }

        public static List<EmployerVacancy> GetEmployerVacancies(string userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Select * from " + TBL_EMPLOYER_JOBCREATED + " where userId=@userId and visibility=1";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<EmployerVacancy> vacancies = new List<EmployerVacancy>();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                EmployerVacancy vacancy = new EmployerVacancy
                                {
                                    userId = reader["userId"].ToString(),
                                    employerVacancyId = reader["employerJobId"].ToString(),
                                    jobType = GetJobTypeName(reader["jobType"].ToString()),
                                    jobTitle = reader["jobTitle"].ToString(),
                                    startDate = Convert.ToDateTime(reader["startDate"]),
                                    endDate = Convert.ToDateTime(reader["endDate"]),
                                    vacancyStatus = reader["vacancyStatus"].ToString(),
                                    locations = GetAvailLocations(reader["employerJobId"].ToString()),
                                    employer = GetEmployerViaUID(reader["userId"].ToString())
                                };
                                vacancies.Add(vacancy);
                            }
                            return vacancies;
                        }
                        else
                        {
                            return new List<EmployerVacancy>();
                        }
                    }
                }
            }
        }

        public static string GetJobTypeName(string jobType)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Select title from " + TBL_JOB_POOL + " where jobId=@jobType";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@jobType", jobType);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            return reader["title"].ToString();
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public static List<JobLocation> GetAvailLocations(string jobId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd1 = "Select * from " + TBL_JOB_LOCATION_AVAILABILITY + " where jobId=@jobId";

                using (SqlCommand command1 = new SqlCommand(cmd1, connection))
                {
                    command1.Parameters.AddWithValue("@jobId", jobId);
                    using (SqlDataReader reader1 = command1.ExecuteReader())
                    {
                        if (reader1.HasRows)
                        {
                            List<JobLocationAvailability> availableLocIds = new List<JobLocationAvailability>();
                            while (reader1.Read())
                            {
                                JobLocationAvailability availableLocId = new JobLocationAvailability
                                {
                                    date = Convert.ToDateTime(reader1["date"]),
                                    jobId = reader1["jobId"].ToString(),
                                    locationId = Convert.ToInt32(reader1["locationId"])
                                };
                                availableLocIds.Add(availableLocId);
                            }
                            reader1.Close();

                            List<JobLocation> jobLocations = new List<JobLocation>();
                            for (int i = 0; i < availableLocIds.Count; i++)
                            {
                                string cmd2 = "Select * from " + TBL_JOB_LOCATIONS + " where locationId=@locationId";
                                using (SqlCommand command2 = new SqlCommand(cmd2, connection))
                                {
                                    command2.Parameters.AddWithValue("@locationId", availableLocIds[i].locationId);
                                    using (SqlDataReader reader2 = command2.ExecuteReader())
                                    {
                                        reader2.Read();
                                        JobLocation jobLocation = new JobLocation
                                        {
                                            dateCreated = Convert.ToDateTime(reader2["dateCreated"]),
                                            locationId = Convert.ToInt32(reader2["locationId"]),
                                            locationName = reader2["locationName"].ToString()
                                        };
                                        jobLocations.Add(jobLocation);
                                    }
                                }
                            }
                            return jobLocations;
                        }
                        else
                        {
                            return new List<JobLocation>();
                        }
                    }
                }
            }
        }

        public static void NewSeeker(SeekerAccount account)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Insert into Seeker (userId, dateCreated, trn, firstName, middleName, lastName, email, isGraduate, isVerified, identifier, employmentStatus) " +
                              "values (@userId, @dateCreated, @trn, @firstName, @middleName, @lastName, @email, @isGraduate, @isVerified, @identifier, @employmentStatus)";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@identifier", account.identifier);
                    command.Parameters.AddWithValue("@userId", Miscellaneous.GenerateUserID());
                    command.Parameters.AddWithValue("@dateCreated", DateTime.Now);
                    command.Parameters.AddWithValue("@trn", account.trn);
                    command.Parameters.AddWithValue("@firstName", account.firstName);
                    command.Parameters.AddWithValue("@middleName", account.middleName);
                    command.Parameters.AddWithValue("@lastName", account.lastName);
                    command.Parameters.AddWithValue("@email", account.email);
                    command.Parameters.AddWithValue("@isGraduate", account.isGraduate);
                    command.Parameters.AddWithValue("@isVerified", account.isVerified);
                    command.Parameters.AddWithValue("@employmentStatus", account.employmentStatus);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void NewEmployer(EmployerAccount account)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string userId = Miscellaneous.GenerateUserID();
                string cmd1 = "Insert into " + TBL_EMPLOYER + " (userId, dateCreated, username, email, firstName, lastName, companyOptional, address, telephone, companyAssociated, accountType)" +
                                                             " values(@userId, @dateCreated, @username, @email, @firstName, @lastName, @companyOptional, @address, @telephone, @companyAssociated, @accountType)";

                string cmd2 = "Insert into " + TBL_CREDENTIALS + " (userId, lastAccessed, hash) values (@userId, @lastAccessed, @hash)";

                using (SqlCommand command1 = new SqlCommand(cmd1, connection))
                {
                    command1.Parameters.AddWithValue("@userId", userId);
                    command1.Parameters.AddWithValue("@dateCreated", DateTime.Now);
                    command1.Parameters.AddWithValue("@username", account.username);
                    command1.Parameters.AddWithValue("@email", account.emailAddress);
                    command1.Parameters.AddWithValue("@firstName", account.firstName);
                    command1.Parameters.AddWithValue("@lastname", account.lastName);
                    command1.Parameters.AddWithValue("@companyOptional", account.companyOptional);
                    command1.Parameters.AddWithValue("@address", account.address);
                    command1.Parameters.AddWithValue("@telephone", account.telephone);
                    command1.Parameters.AddWithValue("@companyAssociated", account.companyAssociated);
                    command1.Parameters.AddWithValue("@accountType", account.accountType);
                    command1.ExecuteNonQuery();
                }

                using (SqlCommand command2 = new SqlCommand(cmd2, connection))
                {
                    command2.Parameters.AddWithValue("@userId", userId);
                    command2.Parameters.AddWithValue("@lastAccessed", SqlDateTime.MinValue);
                    command2.Parameters.AddWithValue("@hash", account.password);
                    command2.ExecuteNonQuery();
                }
            }
        }

        public static Seeker GetSeekerViaUsername(string userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "select seeker.userId, dateCreated, trn, firstName, middleName, lastName, email, isGraduate, isVerified, identifier, lastAccessed, hash, username from seeker " +
                             "left join Credentials on seeker.userId = Credentials.userId where seeker.username = @username";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@username", userId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            DateTime _lastAccessed;
                            string _password;

                            if (reader["lastAccessed"] == DBNull.Value)
                            {
                                _lastAccessed = DateTime.Now; ;
                            }
                            else
                            {
                                _lastAccessed = Convert.ToDateTime(reader["lastAccessed"]);
                            }

                            if (reader["hash"] == DBNull.Value)
                            {
                                _password = "";
                            }
                            else
                            {
                                _password = reader["hash"].ToString();
                            }

                            Seeker seeker = new Seeker
                            {
                                userId = reader["userId"].ToString(),
                                dateCreated = Convert.ToDateTime(reader["dateCreated"]),
                                trn = reader["trn"].ToString(),
                                firstName = reader["firstName"].ToString(),
                                middleName = reader["middleName"].ToString(),
                                lastName = reader["lastName"].ToString(),
                                email = reader["email"].ToString(),
                                isGraduate = Convert.ToBoolean(reader["isGraduate"]),
                                isVerified = Convert.ToBoolean(reader["isVerified"]),
                                lastAccessed = _lastAccessed,
                                identifier = reader["identifier"].ToString(),
                                password = _password,
                                locationsAvailable = GetSeekerLocationsAvailability(reader["userId"].ToString()),
                                jobTypesInterested = GetSeekerJobTypeInterests(reader["userId"].ToString()),
                                username = reader["username"].ToString()
                            };
                            return seeker;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public static Seeker GetSeekerViaUID(string userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "select seeker.userId, dateCreated, trn, firstName, middleName, lastName, email, isGraduate, isVerified, identifier, lastAccessed, hash, username from seeker " +
                             "left join Credentials on seeker.userId = Credentials.userId where seeker.userId = @userId";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            DateTime _lastAccessed;
                            string _password;

                            if (reader["lastAccessed"] == DBNull.Value)
                            {
                                _lastAccessed = DateTime.Now; ;
                            }
                            else
                            {
                                _lastAccessed = Convert.ToDateTime(reader["lastAccessed"]);
                            }

                            if (reader["hash"] == DBNull.Value)
                            {
                                _password = "";
                            }
                            else
                            {
                                _password = reader["hash"].ToString();
                            }

                            Seeker seeker = new Seeker
                            {
                                userId = reader["userId"].ToString(),
                                dateCreated = Convert.ToDateTime(reader["dateCreated"]),
                                trn = reader["trn"].ToString(),
                                firstName = reader["firstName"].ToString(),
                                middleName = reader["middleName"].ToString(),
                                lastName = reader["lastName"].ToString(),
                                email = reader["email"].ToString(),
                                isGraduate = Convert.ToBoolean(reader["isGraduate"]),
                                isVerified = Convert.ToBoolean(reader["isVerified"]),
                                lastAccessed = _lastAccessed,
                                identifier = reader["identifier"].ToString(),
                                password = _password,
                                locationsAvailable = GetSeekerLocationsAvailability(reader["userId"].ToString()),
                                jobTypesInterested = GetSeekerJobTypeInterests(reader["userId"].ToString()),
                                username = reader["username"].ToString()
                            };
                            return seeker;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public static Seeker GetSeekerViaIdentifier(string identifier)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "select seeker.userId, dateCreated, trn, firstName, middleName, lastName, email, isGraduate, isVerified, identifier, lastAccessed, hash, username from seeker " +
                             "left join Credentials on seeker.userId = Credentials.userId where seeker.identifier = @identifier";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@identifier", identifier);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            DateTime _lastAccessed;
                            string _password;

                            if (reader["lastAccessed"] == DBNull.Value)
                            {
                                _lastAccessed = DateTime.Now; ;
                            }
                            else
                            {
                                _lastAccessed = Convert.ToDateTime(reader["lastAccessed"]);
                            }

                            if (reader["hash"] == DBNull.Value)
                            {
                                _password = "";
                            }
                            else
                            {
                                _password = reader["hash"].ToString();
                            }

                            Seeker seeker = new Seeker
                            {
                                userId = reader["userId"].ToString(),
                                dateCreated = Convert.ToDateTime(reader["dateCreated"]),
                                trn = reader["trn"].ToString(),
                                firstName = reader["firstName"].ToString(),
                                middleName = reader["middleName"].ToString(),
                                lastName = reader["lastName"].ToString(),
                                email = reader["email"].ToString(),
                                isGraduate = Convert.ToBoolean(reader["isGraduate"]),
                                isVerified = Convert.ToBoolean(reader["isVerified"]),
                                lastAccessed = _lastAccessed,
                                identifier = reader["identifier"].ToString(),
                                password = _password,
                                locationsAvailable = GetSeekerLocationsAvailability(reader["userId"].ToString()),
                                jobTypesInterested = GetSeekerJobTypeInterests(reader["userId"].ToString()),
                                username = reader["username"].ToString()
                            };
                            return seeker;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public static List<Seeker> GetAllSeekers(int limit)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "select top " + limit + " seeker.userId, (firstName+' '+lastName) as name, dateCreated, trn, firstName, middleName, lastName, email, isGraduate, isVerified, identifier, employmentStatus, hash, username from seeker left join Credentials on Seeker.userId=Credentials.userId order by name asc";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    List<Seeker> seekers = new List<Seeker>();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string _password;
                                if (reader["hash"] == null)
                                {
                                    _password = "";
                                }
                                else
                                {
                                    _password = reader["hash"].ToString();
                                }

                                Seeker seeker = new Seeker
                                {
                                    userId = reader["userId"].ToString(),
                                    dateCreated = Convert.ToDateTime(reader["dateCreated"]),
                                    trn = reader["trn"].ToString(),
                                    firstName = reader["firstName"].ToString(),
                                    middleName = reader["middleName"].ToString(),
                                    lastName = reader["lastName"].ToString(),
                                    email = reader["email"].ToString(),
                                    isGraduate = Convert.ToBoolean(reader["isGraduate"]),
                                    isVerified = Convert.ToBoolean(reader["isVerified"]),
                                    identifier = reader["identifier"].ToString(),
                                    employmentStatus = reader["employmentStatus"].ToString(),
                                    password = _password,
                                    locationsAvailable = GetSeekerLocationsAvailability(reader["userId"].ToString()),
                                    jobTypesInterested = GetSeekerJobTypeInterests(reader["userId"].ToString()),
                                    username = reader["username"].ToString()
                                };
                                seekers.Add(seeker);
                            }
                            return seekers;
                        }
                        else
                        {
                            return new List<Seeker>();
                        }
                    }
                }
            }
        }

        public static Employer GetEmployerViaUID(string userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string cmd = "select " + TBL_EMPLOYER + ".userId, dateCreated, username, email, firstName, lastName, companyOptional, address, telephone, companyAssociated, accountType, lastAccessed, hash from " + TBL_EMPLOYER + " " +
                             "inner join " + TBL_CREDENTIALS + " " +
                             "on " + TBL_EMPLOYER + ".userId = " + TBL_CREDENTIALS + ".userId where " + TBL_EMPLOYER + ".userId = @userId";
                connection.Open();

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            Employer employer = new Employer
                            {
                                userId = reader["userId"].ToString(),
                                dateCreated = Convert.ToDateTime(reader["dateCreated"]),
                                username = reader["username"].ToString(),
                                email = reader["email"].ToString(),
                                firstName = reader["firstName"].ToString(),
                                lastName = reader["lastName"].ToString(),
                                companyOptional = reader["companyOptional"].ToString(),
                                address = reader["address"].ToString(),
                                telephone = reader["telephone"].ToString(),
                                companyAssociatedId = reader["companyAssociated"].ToString(),
                                companyAssociatedName = GetCompany(reader["companyAssociated"].ToString()).companyName,
                                accountType = reader["accountType"].ToString(),
                                password = reader["hash"].ToString()
                            };
                            employer.SetCompanyName();
                            return employer;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public static Employer GetEmployerViaUsername(string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string cmd = "select " + TBL_EMPLOYER + ".userId, dateCreated, username, email, firstName, lastName, companyOptional, address, telephone, companyAssociated, accountType, lastAccessed, hash from " + TBL_EMPLOYER + " " +
                             "inner join " + TBL_CREDENTIALS + " " +
                             "on " + TBL_EMPLOYER + ".userId = " + TBL_CREDENTIALS + ".userId where " + TBL_EMPLOYER + ".username = @username";
                connection.Open();

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            Employer employer = new Employer
                            {
                                userId = reader["userId"].ToString(),
                                dateCreated = Convert.ToDateTime(reader["dateCreated"]),
                                username = reader["username"].ToString(),
                                email = reader["email"].ToString(),
                                firstName = reader["firstName"].ToString(),
                                lastName = reader["lastName"].ToString(),
                                companyOptional = reader["companyOptional"].ToString(),
                                address = reader["address"].ToString(),
                                telephone = reader["telephone"].ToString(),
                                companyAssociatedId = reader["companyAssociated"].ToString(),
                                companyAssociatedName = GetCompany(reader["companyAssociated"].ToString()).companyName,
                                accountType = reader["accountType"].ToString(),
                                password = reader["hash"].ToString()
                            };
                            employer.SetCompanyName();
                            return employer;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public static List<Company> GetCompanies()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string cmd = "Select * from " + TBL_COMPANIES + " where companyId != '0' order by companyName asc";
                connection.Open();

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Company> companies = new List<Company>();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Company company = new Company
                                {
                                    companyId = reader["companyId"].ToString(),
                                    companyName = reader["companyName"].ToString(),
                                    createdDate = Convert.ToDateTime(reader["dateCreated"])
                                };
                                companies.Add(company);
                            }
                            return companies;
                        }
                        else
                        {
                            return new List<Company>();
                        }
                    }
                }
            }
        }

        public static string AddCompany(Company company)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Insert into Companies (companyId, companyName, address, dateCreated) values (@companyId, @companyName, @address, @dateCreated)";
                string companyId = Miscellaneous.GenerateCompanyId();

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@companyId", companyId);
                    command.Parameters.AddWithValue("@companyName", company.companyName);
                    command.Parameters.AddWithValue("@address", company.address);
                    command.Parameters.AddWithValue("@dateCreated", DateTime.Now);
                    command.ExecuteNonQuery();
                    return companyId;
                }
            }
        }

        public static Company GetCompany(string companyId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string cmd = "Select * from " + TBL_COMPANIES + " where companyId=@companyId";
                connection.Open();

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@companyId", companyId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            Company company = new Company
                            {
                                companyId = reader["companyId"].ToString(),
                                companyName = reader["companyName"].ToString(),
                                createdDate = Convert.ToDateTime(reader["dateCreated"])
                            };
                            return company;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public static List<Seeker> GetSeekersByName(string name)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "select t.userId, t.dateCreated, t.trn, t.firstName, t.middleName, t.lastName, t.email, t.isGraduate, t.isVerified, t.identifier, t.name, t.employmentStatus, t.username from (select userId,dateCreated,trn,firstName,middleName,lastName,email,isGraduate,isVerified,identifier,employmentStatus,username, (firstName+' '+lastName) as name from Seeker) as t where t.name like '%" + name + "%'";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Seeker> seekers = new List<Seeker>();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Seeker seeker = new Seeker
                                {
                                    userId = reader["userId"].ToString(),
                                    dateCreated = Convert.ToDateTime(reader["dateCreated"]),
                                    trn = reader["trn"].ToString(),
                                    firstName = reader["firstName"].ToString(),
                                    middleName = reader["middleName"].ToString(),
                                    lastName = reader["lastName"].ToString(),
                                    email = reader["email"].ToString(),
                                    isGraduate = Convert.ToBoolean(reader["isGraduate"]),
                                    isVerified = Convert.ToBoolean(reader["isVerified"]),
                                    identifier = reader["identifier"].ToString(),
                                    employmentStatus = reader["employmentStatus"].ToString(),
                                    locationsAvailable = GetSeekerLocationsAvailability(reader["userId"].ToString()),
                                    jobTypesInterested = GetSeekerJobTypeInterests(reader["userId"].ToString()),
                                    username = reader["username"].ToString()
                                };

                                seeker.locationsList = seeker.GetFormattedJobLocations();
                                seeker.jobTypesList = seeker.GetFormattedJobTypes();
                                seeker.fullname = seeker.firstName + " " + seeker.lastName;
                                seekers.Add(seeker);
                            }
                            return seekers;
                        }
                        else
                        {
                            return new List<Seeker>();
                        }
                    }
                }
            }
        }

        public static bool CheckCompanyExist(string companyName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Select count(*) as count from Companies where companyName=@companyName";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@companyName", companyName);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        if (Convert.ToInt32(reader["count"]) > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        public static bool CheckEmployerExist(string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Select username from " + TBL_EMPLOYER + " where username=@username";
                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        public static bool CheckSeekerExistViaId(string identifier)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Select identifier from " + TBL_SEEKER + " where identifier=@identifier";
                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@identifier", identifier);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        public static bool CheckSeekerExistViaUname(string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Select username from " + TBL_SEEKER + " where username=@username";
                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        public static void NewSeekerActivationCode(string userId, string activationCode, DateTime validUntil)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Insert into SeekerAccountActivation (userId, dateRequested, validUntil, activationCode, status) " +
                                                          "values (@userId, @dateRequested, @validUntil, @activationCode, @status)";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@userId", @userId);
                    command.Parameters.AddWithValue("@dateRequested", DateTime.Now);
                    command.Parameters.AddWithValue("@validUntil", validUntil);
                    command.Parameters.AddWithValue("@activationCode", activationCode);
                    command.Parameters.AddWithValue("@status", ACTIVATION_STATUS_INCOMPLETE);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void CompleteSeekerActivation(string userId, string activationCode, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Update SeekerAccountActivation set status=@status where activationCode=@activationCode; " +
                             "Update Seeker set isVerified = 'true' where userId=@userId; " +
                             "declare @count int; " +
                             "set @count = (select count(userId) from Credentials where userId = @userId) " +
                             "if (@count > 0) " +
                             "                  begin " +
                             "                      update Credentials set hash = @hash where userId = @userId " +
                             "end " +
                             "else " +
                             "                   begin " +
                             "                       insert into Credentials(userId, lastAccessed, hash) values(@userId, @lastAccessed, @hash) " +
                            "end";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@status", ACTIVATION_STATUS_COMPLETED);
                    command.Parameters.AddWithValue("@activationCode", activationCode);
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@lastAccessed", DateTime.Now);
                    command.Parameters.AddWithValue("@hash", password);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static SeekerActivation GetActivation(string activationCode)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Select * from SeekerAccountActivation where activationCode=@activationCode";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@activationCode", activationCode);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            SeekerActivation activation = new SeekerActivation
                            {
                                userId = reader["userId"].ToString(),
                                dateRequested = Convert.ToDateTime(reader["dateRequested"]),
                                validUntil = Convert.ToDateTime(reader["validUntil"]),
                                activationCode = reader["activationCode"].ToString(),
                                status = reader["status"].ToString()
                            };
                            return activation;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public static int GetVacancyCount()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Select count(*) as count from Employer_JobCreated where visibility=1";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        return Convert.ToInt32(reader["count"]);
                    }
                }
            }
        }

        public static int GetSeekerJobCount()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Select count(*) as count from Seeker_JobCreated";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        return Convert.ToInt32(reader["count"]);
                    }
                }
            }
        }

        public static int GetJobPoolCount()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Select count(*) as count from JobPool";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        return Convert.ToInt32(reader["count"]);
                    }
                }
            }
        }

        public static int GetCompanyCount()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Select count(*) as count from Companies";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        return Convert.ToInt32(reader["count"]);
                    }
                }
            }
        }

        public static void AddJobToPool(string title)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Insert into JobPool (jobId, dateCreated, title) values (@jobId, @dateCreated, @title)";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@jobId", Miscellaneous.GenerateJobPoolId());
                    command.Parameters.AddWithValue("@dateCreated", DateTime.Now);
                    command.Parameters.AddWithValue("@title", title);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static Job GetJobTypeForEmployerJob(string jobId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "select emp.jobType, emp.dateCreated, job.title from Employer_JobCreated emp inner join " +
                             "JobPool job on emp.jobType = job.jobId where emp.employerJobId = @jobId and visibility=1";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@jobId", jobId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            Job job = new Job
                            {
                                jobId = reader["jobType"].ToString(),
                                dateCreated = Convert.ToDateTime(reader["dateCreated"]),
                                title = reader["title"].ToString()
                            };
                            return job;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public static List<JobLocation> GetLocationsForJob(string jobId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Select jobAvail.locationId, jobAvail.jobId, jobAvail.date, jobLoc.locationName from Job_LocationAvailability jobAvail inner join " +
                             "Job_Locations jobLoc on jobAvail.locationId = jobLoc.locationId where jobAvail.jobId = @jobId";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    List<JobLocation> locations = new List<JobLocation>();
                    command.Parameters.AddWithValue("@jobId", jobId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                JobLocation location = new JobLocation
                                {
                                    dateCreated = Convert.ToDateTime(reader["date"]),
                                    locationId = Convert.ToInt32(reader["locationId"]),
                                    locationName = reader["locationName"].ToString()
                                };
                                locations.Add(location);
                            }
                            return locations;
                        }
                        else
                        {
                            return new List<JobLocation>();
                        }
                    }
                }
            }
        }

        public static EmployerVacancy CreateVacancy(EmployerVacancy vacancy)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Insert into Employer_JobCreated (userId, employerJobId, jobType, jobTitle, startDate, endDate, vacancyStatus, dateCreated) " +
                                                      "values (@userId, @employerJobId, @jobType, @jobTitle, @startDate, @endDate, @vacancyStatus, @dateCreated)";

                string employerJobId = Miscellaneous.GenerateVacancyId();
                vacancy.vacancyStatus = VACANCY_STATUS_OPEN;
                vacancy.dateCreated = DateTime.Now;
                vacancy.employerVacancyId = employerJobId;

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@userId", vacancy.userId);
                    command.Parameters.AddWithValue("@employerJobId", employerJobId);
                    command.Parameters.AddWithValue("@jobType", vacancy.jobType);
                    command.Parameters.AddWithValue("@jobTitle", vacancy.jobTitle);
                    command.Parameters.AddWithValue("@startDate", vacancy.startDate);
                    command.Parameters.AddWithValue("@endDate", vacancy.endDate);
                    command.Parameters.AddWithValue("@vacancyStatus", vacancy.vacancyStatus);
                    command.Parameters.AddWithValue("@dateCreated", vacancy.dateCreated);
                    command.ExecuteNonQuery();
                    StoreJobLocations(vacancy.locations, employerJobId);
                    return vacancy;
                }
            }
        }

        public static void StoreJobLocations(List<JobLocation> jobLocations, string jobId)
        {
            if (jobLocations != null)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string cmd = "Insert into Job_LocationAvailability (jobId, locationId, date) values (@jobId, @locationId, @date)";

                    foreach (JobLocation jobLocation in jobLocations)
                    {
                        using (SqlCommand command = new SqlCommand(cmd, connection))
                        {
                            command.Parameters.AddWithValue("@jobId", jobId);
                            command.Parameters.AddWithValue("@locationId", jobLocation.locationId);
                            command.Parameters.AddWithValue("@date", DateTime.Now);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }


        public static void UpdateJobLocations(List<JobLocation> jobLocations, string jobId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Delete from Job_LocationAvailability where jobId=@jobId";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@jobId", jobId);
                    command.ExecuteNonQuery();
                }
                StoreJobLocations(jobLocations, jobId);
            }
        }

        public static EmployerVacancy UpdateJobVacancy(EmployerVacancy vacancy)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Update Employer_JobCreated set jobType=@jobType, jobTitle=@jobTitle, endDate=@endDate, vacancyStatus=@vacancyStatus where employerJobId=@jobId";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@jobId", vacancy.employerVacancyId);
                    command.Parameters.AddWithValue("@jobType", vacancy.jobType);
                    command.Parameters.AddWithValue("@jobTitle", vacancy.jobTitle);
                    command.Parameters.AddWithValue("@endDate", vacancy.endDate);
                    command.Parameters.AddWithValue("@vacancyStatus", vacancy.vacancyStatus);
                    command.ExecuteNonQuery();
                    UpdateJobLocations(vacancy.locations, vacancy.employerVacancyId);
                    return vacancy;
                }
            }
        }

        public static void SetJobVacancyInvisible(string jobId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Update Employer_JobCreated set visibility='false' where employerJobId=@jobId;";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@jobId", jobId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void CreateSeekerJob(SeekerJobCreated seekerJob)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Insert into Seeker_JobCreated (userId, dateCreated, jobType, seekerJobId, isSeeking, jobName)";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@userId", seekerJob.userId);
                    command.Parameters.AddWithValue("@dateCreated", DateTime.Now);
                    command.Parameters.AddWithValue("@jobType", seekerJob.jobType);
                    command.Parameters.AddWithValue("@seekerJobId", Miscellaneous.GenerateSeekerJobId());
                    command.Parameters.AddWithValue("@isSeeking", seekerJob.isSeeking);
                    command.Parameters.AddWithValue("@jobName", seekerJob.jobName);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static List<Job> GetJobTypes()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Select * from JobPool order by title asc";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Job> jobs = new List<Job>();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Job job = new Job
                                {
                                    dateCreated = Convert.ToDateTime(reader["dateCreated"]),
                                    jobId = reader["jobId"].ToString(),
                                    title = reader["title"].ToString()
                                };
                                jobs.Add(job);
                            }
                            return jobs;
                        }
                        else
                        {
                            return new List<Job>();
                        }
                    }
                }
            }
        }

        public static List<JobLocation> GetJobLocations()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Select * from Job_Locations order by locationName desc";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<JobLocation> jobLocations = new List<JobLocation>();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                JobLocation jobLocation = new JobLocation
                                {
                                    dateCreated = Convert.ToDateTime(reader["dateCreated"]),
                                    locationId = Convert.ToInt32(reader["locationId"]),
                                    locationName = reader["locationName"].ToString()
                                };
                                jobLocations.Add(jobLocation);
                            }
                            return jobLocations;
                        }
                        else
                        {
                            return new List<JobLocation>();
                        }
                    }
                }
            }
        }

        public static bool CheckVacancyExist(string jobTitle)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Select jobTitle  from Employer_JobVacany where jobTitle=@jobTitle";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (Convert.ToInt32(reader["count"]) > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        public static void AddVacancyLocation(string employerJobId, int locationId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Insert into Employer_JobLocations (employerJobId, locationId) values (@employerJobId, @locationId)";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@employerJobId", employerJobId);
                    command.Parameters.AddWithValue("@locationId", locationId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}