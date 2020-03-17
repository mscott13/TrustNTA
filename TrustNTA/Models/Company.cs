using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrustNTA.Models
{
    public class Company
    {
        public string companyId { get; set; }
        public string companyName { get; set; }
        public string address { get; set; }
        public DateTime createdDate { get; set; }
    }
}