using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrustNTA.Models
{
    public class Credentials
    {
        public string userId { get; set; }
        public DateTime lastAccessed { get; set; }
        public string accessKey { get; set; }
    }
}