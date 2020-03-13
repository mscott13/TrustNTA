using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrustNTA.Models
{
    public class Company
    {
        public int companyId { get; set; }
        public string companyName { get; set; }
        public DateTime createdDate { get; set; }
    }
}