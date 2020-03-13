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

        public static List<EmployerVacancy> GetEmployerVacancies(string userId) 
        {
            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Select * from "+TBL_EMPLOYER_JOBCREATED+" where userId=@userId";

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
                                    locations = GetAvailLocations(reader["employerJobId"].ToString())
                                };
                                vacancies.Add(vacancy);
                            }
                            return vacancies;
                        }
                        else 
                        {
                            return null;
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
                string cmd = "Select title from "+TBL_JOB_POOL+" where jobId=@jobType";

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
            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd1 = "Select * from "+TBL_JOB_LOCATION_AVAILABILITY+" where jobId=@jobId";

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
                            for(int i=0; i<availableLocIds.Count; i++) 
                            {
                                string cmd2 = "Select * from "+TBL_JOB_LOCATIONS+" where locationId=@locationId";
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
                            return null;
                        }
                    }
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

        public static Employer GetEmployer(string userId) 
        {
            using (SqlConnection connection = new SqlConnection(connectionString)) 
            {
                string cmd = "select "+TBL_EMPLOYER+".userId, dateCreated, username, email firstName, lastName, companyOptional, address, telephone, companyAssociated, accountType, hash from "+TBL_EMPLOYER+ " "+
                             "inner join "+TBL_CREDENTIALS+" "+
                             "on "+TBL_EMPLOYER+".userId = "+TBL_CREDENTIALS+".userId where "+TBL_EMPLOYER+".userId = @userId";
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
                                companyAssociated = Convert.ToInt32(reader["companyAssociated"]),
                                accountType = reader["accountType"].ToString(),
                                password = reader["password"].ToString()
                            };
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
                string cmd = "Select * from " + TBL_COMPANIES;
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
                                    companyId = Convert.ToInt32(reader["companyId"]),
                                    companyName = reader["companyName"].ToString(),
                                    createdDate = Convert.ToDateTime(reader["createdDate"])
                                };
                                companies.Add(company);
                            }
                            return companies;
                        }
                        else 
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public static Company GetCompany(int companyId) 
        {
            using (SqlConnection connection = new SqlConnection(connectionString)) 
            {
                string cmd = "Select * from "+TBL_COMPANIES+" where companyId=@companyId";
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
                                companyId = Convert.ToInt32(reader["companyId"]),
                                companyName = reader["companyName"].ToString(),
                                createdDate = Convert.ToDateTime(reader["createdDate"])
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

        public static bool CheckEmployerUserExist(string username) 
        {
            using(SqlConnection connection = new SqlConnection(connectionString)) 
            {
                connection.Open();
                string cmd = "Select username from "+TBL_EMPLOYER+" where username=@username";
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
    }
}