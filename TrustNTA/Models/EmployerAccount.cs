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

        private string _accountType;
        private string _firstName;
        private string _lastName;
        private string _companyOptional;
        private string _address;
        private string _emailAddress;
        private string _telephone;
        private string _username;
        private string _password;
        public string accountType 
        {
            get { return _accountType; }
            set 
            {
                if (value == null)
                {
                    _accountType = "";
                }
                else 
                {
                    _accountType = value;
                }
            }
        }
        public string firstName 
        {
            get { return _firstName; }
            set
            {
                if (value == null)
                {
                    _firstName = "";
                }
                else
                {
                    _firstName = value;
                }
            }
        }
        public string lastName 
        {
            get { return _lastName; }
            set
            {
                if (value == null)
                {
                    _lastName = "";
                }
                else
                {
                    _lastName = value;
                }
            }
        }
        public int companyAssociated { get; set; }
        public string companyOptional {
            get { return _companyOptional; }
            set
            {
                if (value == null)
                {
                    _companyOptional = "";
                }
                else
                {
                    _companyOptional = value;
                }
            }
        }
        public string address
        {
            get { return _address; }
            set
            {
                if (value == null)
                {
                    _address = "";
                }
                else
                {
                    _address = value;
                }
            }
        }
        public string emailAddress
        {
            get { return _emailAddress; }
            set
            {
                if (value == null)
                {
                    _emailAddress = "";
                }
                else
                {
                    _emailAddress = value;
                }
            }
        }
        public string telephone
        {
            get { return _telephone; }
            set
            {
                if (value == null)
                {
                    _telephone = "";
                }
                else
                {
                    _telephone = value;
                }
            }
        }
        public string username {
            get { return _username; }
            set
            {
                if (value == null)
                {
                    _username = "";
                }
                else
                {
                    _username = value;
                }
            }
        }
       
        public string password 
        {
            get { return _password; }
            set { _password = PasswordManagement.HashPassword(value); }
        }
    }
}