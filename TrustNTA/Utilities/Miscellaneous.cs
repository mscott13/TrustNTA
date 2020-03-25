using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrustNTA.Data_Access;
using RestSharp;
using RestSharp.Authenticators;
using System.Security.Cryptography;

namespace TrustNTA.Utilities
{
    public static class Miscellaneous
    {
        public static string GenerateActivationCode() 
        {
            using (SHA512 code = new SHA512Managed()) 
            {
                return  Convert.ToBase64String(code.ComputeHash(System.Text.Encoding.UTF8.GetBytes(DateTime.Now.ToString())));
            }
        }

        public static string GenerateVacancyId() 
        {
            return Database.GetVacancyCount().ToString() + "empl_" + Guid.NewGuid().ToString().Substring(0, 9); ;
        }

        public static string GenerateSeekerJobId()
        {
            return Database.GetSeekerJobCount().ToString() + "seek_" + Guid.NewGuid().ToString().Substring(0, 9); ;
        }

        public static string GenerateJobPoolId()
        {
            return Database.GetSeekerJobCount().ToString() + "job_" + Guid.NewGuid().ToString().Substring(0, 4); ;
        }

        public static string GenerateCompanyId()
        {
            return Database.GetCompanyCount().ToString() + "_" + Guid.NewGuid().ToString().Substring(0, 6); ;
        }

        public static string GenerateResumePrefix()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssffff") + "-" + Guid.NewGuid().ToString().Substring(0, 8);
        }


        public static string GenerateUserID() 
        {
            return Guid.NewGuid().ToString();
        }

        public static IRestResponse SendEmail() 
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
           new HttpBasicAuthenticator("api",
                                       "d147946c955d9901086b1405ce6c1753-ee13fadb-4c9a714d");
            RestRequest request = new RestRequest();
            request.AddParameter("domain", "https://api.mailgun.net/v3/sandboxe03efb7190484ac29eb6741821e3f21d.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Excited User <mailgun@sandboxe03efb7190484ac29eb6741821e3f21d.mailgun.org>");
            request.AddParameter("to", "a.markscott13@gmail.com");
            request.AddParameter("subject", "Hello");
            request.AddParameter("text", "Testing some Mailgun awesomness!");
            request.Method = Method.POST;
            return client.Execute(request);
        }
    }
}