using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrustNTA.Data_Access;

namespace TrustNTA.Models
{
    public class Employer
    {
        public Employer() { }
        public string userId { get; set; }
        public DateTime dateCreated { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string companyOptional { get; set; }
        public string address { get; set; }
        public string telephone { get; set; }
        public string companyAssociatedId { get; set; }
        public string companyAssociatedName { get; set; }
        public string accountType { get; set; }
        public string password { get; set; }
        public string companyName { get; set; }

        public void SetCompanyName() 
        {
            if (accountType == Database.EMPLOYER_ACCOUNT_COMPANY)
            {
                companyName = companyAssociatedName;
            }
            else 
            {
                if (companyOptional == null || companyOptional == "")
                {
                    companyName = firstName + " " + lastName;
                }
                else 
                {
                    companyName = companyOptional;
                }
            }
        } 
    }
}