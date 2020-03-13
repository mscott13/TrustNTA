using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrustNTA.Models
{
    public class JobLocation
    {
        public int locationId { get; set; }
        public string locationName { get; set; }
        public DateTime dateCreated { get; set; }
    }
}