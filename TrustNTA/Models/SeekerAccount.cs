using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrustNTA.Models
{
    public class SeekerAccount
    {
        private string _userId;
        private DateTime _dateCreated;
        private string _trn;
        private string _firstName;
        private string _middleName;
        private string _lastName;
        private string _email;
        private string _identifier;
        private string _employmentStatus;

        public bool isVerified { get; set; }
        public bool isGraduate { get; set; }
        public string identifier
        {
            get { return _identifier; }
            set
            {
                if (value == null)
                {
                    _identifier = "";
                }
                else
                {
                    _identifier = value;
                }
            }
        }
        public string employmentStatus
        {
            get { return _employmentStatus; }
            set
            {
                if (value == null)
                {
                    _employmentStatus = "";
                }
                else
                {
                    _employmentStatus = value;
                }
            }
        }
        public string email
        {
            get { return _email; }
            set
            {
                if (value == null)
                {
                    _email = "";
                }
                else
                {
                    _email = value;
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
        public string middleName
        {
            get { return _middleName; }
            set
            {
                if (value == null)
                {
                    _middleName = "";
                }
                else
                {
                    _middleName = value;
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
        public string trn
        {
            get { return _trn; }
            set
            {
                if (value == null)
                {
                    _trn = "";
                }
                else
                {
                    _trn = value;
                }
            }
        }
        public string userId
        {
            get { return _userId; }
            set
            {
                if (value == null)
                {
                    _userId = "";
                }
                else
                {
                    _userId = value;
                }
            }
        }
        public DateTime dateCreated
        {
            get { return _dateCreated; }
            set
            {
                if (value == null)
                {
                    _dateCreated = DateTime.Now; ;
                }
                else
                {
                    _dateCreated = value;
                }
            }
        }
    }
}