﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrustNTA.Models
{
    public class Employer
    {
        public string userId { get; set; }
        public DateTime dateCreated { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string companyOptional { get; set; }
        public string address { get; set; }
        public string telephone { get; set; }
        public int companyAssociated { get; set; }
        public string accountType { get; set; }
        public string password { get; set; }
    }
}