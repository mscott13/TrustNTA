using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrustNTA.Models
{
    public class SeekerJobInterested
    {
        public string userId { get; set; }
        public DateTime dateCreated { get; set; }
        public string jobType { get; set; }
        public string employerJobId { get; set; }
    }
}