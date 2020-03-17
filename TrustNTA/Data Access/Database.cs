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

        public static EmployerVacancy GetEmployerVacancy(string jobId) 
        {
            using (SqlConnection connection = new SqlConnection(connectionString)) 
            {
                connection.Open();
                string cmd = "Select * from Employer_JobCreated where employerJobId=@jobId";

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
                                dateCreated = Convert.ToDateTime(reader["dateCreated"])
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

        public static List<EmployerVacancy> GetEmployerVacancies(string userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "Select * from " + TBL_EMPLOYER_JOBCREATED + " where userId=@userId";

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
                            return null;
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
                string cmd = "Insert into Seeker (userId, dateCreated, trn, firstName, middleName, lastName, email, isGraduate, isVerified, identifier) " +
                              "values (@userId, @dateCreated, @trn, @firstName, @middleName, @lastName, @email, @isGraduate, @isVerified, @identifier)";

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

        public static Seeker GetSeekerViaUID(string userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "select seeker.userId, dateCreated, trn, firstName, middleName, lastName, email, isGraduate, isVerified, identifier, lastAccessed, hash from seeker " +
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
                                password = _password
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
                string cmd = "select seeker.userId, dateCreated, trn, firstName, middleName, lastName, email, isGraduate, isVerified, identifier, lastAccessed, hash from seeker " +
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
                                password = _password
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

        public static Employer GetEmployerViaUID(string userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string cmd = "select " + TBL_EMPLOYER + ".userId, dateCreated, username, email firstName, lastName, companyOptional, address, telephone, companyAssociated, accountType, lastAccessed, hash from " + TBL_EMPLOYER + " " +
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
                                companyAssociated = Convert.ToInt32(reader["companyAssociated"]),
                                accountType = reader["accountType"].ToString(),
                                password = reader["hash"].ToString()
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
                                companyAssociated = Convert.ToInt32(reader["companyAssociated"]),
                                accountType = reader["accountType"].ToString(),
                                password = reader["hash"].ToString()
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
                            return null;
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

        public static Company GetCompany(int companyId)
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

        public static List<SeekerSearch> GetSeekersByName(string name)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string cmd = "select t.userId, t.dateCreated, t.trn, t.firstName, t.middleName, t.lastName, t.email, t.isGraduate, t.isVerified, t.identifier, t.name from (select userId,dateCreated,trn,firstName,middleName,lastName,email,isGraduate,isVerified,identifier, (firstName+' '+lastName) as name from Seeker) as t where t.name like '%" + name + "%'";

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<SeekerSearch> seekers = new List<SeekerSearch>();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                SeekerSearch seeker = new SeekerSearch
                                {
                                    value = reader["userId"].ToString(),
                                    data = reader["firstName"].ToString() + " " + reader["lastName"].ToString(),
                                };
                                seekers.Add(seeker);
                            }
                            return seekers;
                        }
                        else
                        {
                            return new List<SeekerSearch>();
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

        public static bool CheckSeekerExist(string identifier)
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
                string cmd = "Select count(*) as count from Employer_JobCreated";

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
                             "JobPool job on emp.jobType = job.jobId where emp.employerJobId = @jobId";

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

        
        public static void UpdateJobLocations(List<JobLocation> jobLocations, string jobId) 
        {
            using (SqlConnection connection = new SqlConnection(connectionString)) 
            {
                connection.Open();
                string cmd = "Update Job_LocationAvailability set locationId=@locationid, date=getdate() where jobId=@jobId";

                foreach (JobLocation jobLocation in jobLocations) 
                {
                    using (SqlCommand command = new SqlCommand(cmd, connection))
                    {
                        command.Parameters.AddWithValue("@jobId", jobId);
                        command.Parameters.AddWithValue("@locationId", jobLocation.locationId);
                        command.ExecuteNonQuery();
                    }
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