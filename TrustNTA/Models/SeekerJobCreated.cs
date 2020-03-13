using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrustNTA.Models
{
    public class SeekerJobCreated
    {
        public string userId { get; set; }
        public DateTime dateCreated { get; set; }
        public string jobType { get; set; }
        public  string seekerJobId { get; set; }
        public bool isSeeking { get; set; }
        public string jobName { get; set; }
    }
}