using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrustNTA.Models
{
    public class Seeker
    {
        public string userId { get; set; }
        public DateTime dateCreated { get; set; }
        public string trn { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public bool isGraduate { get; set; }
        public bool isVerified { get; set; }
        public string identifier { get; set; }
        public DateTime lastAccessed { get; set; }
        public string password { get; set; }
    }
}