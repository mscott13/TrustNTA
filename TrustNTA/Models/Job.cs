using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrustNTA.Models
{
    public class Job
    {
        public string jobId { get; set; }
        public DateTime dateCreated { get; set; }
        public string title { get; set; }
    }
}