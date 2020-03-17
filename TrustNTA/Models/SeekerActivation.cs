using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrustNTA.Models
{
    public class SeekerActivation
    {
        private string _userId;
        private string _activationCode;
        private string _status;

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

        public string activationCode
        {
            get { return _activationCode; }
            set
            {
                if (value == null)
                {
                    _activationCode = "";
                }
                else
                {
                    _activationCode = value;
                }
            }
        }

        public string status
        {
            get { return _status; }
            set
            {
                if (value == null)
                {
                    _status = "";
                }
                else
                {
                    _status = value;
                }
            }
        }

        public DateTime dateRequested { get; set; }
        public DateTime validUntil { get; set; }
    }
}