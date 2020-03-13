using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrustNTA.Utilities;

namespace TrustNTA.Models
{
    public class EmployerAccount
    {
        public const string ACCOUNT_TYPE_COMPANY = "COMPANY";
        public const string ACCOUNT_TYPE_INDIVIDUAL = "INDIVIDUAL";

        public string accountType { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int companyAssociated { get; set; }
        public string companyOptional { get; set; }
        public string address { get; set; }
        public string emailAddress { get; set; }
        public string telephone { get; set; }
        public string username { get; set; }
        public string password;
        public string Password 
        {
            get { return password; }
            set { password = PasswordManagement.HashPassword(value); }
        }
    }
}