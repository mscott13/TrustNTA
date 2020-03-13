using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrustNTA.Models
{
    public class JobLocationAvailability
    {
        int entryId { get; set; }
        public string jobId { get; set; }
        public int locationId { get; set; }
        public DateTime date { get; set; }
    }
}