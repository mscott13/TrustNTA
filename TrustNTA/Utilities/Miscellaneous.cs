using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrustNTA.Data_Access;

namespace TrustNTA.Utilities
{
    public static class Miscellaneous
    {
        public static string GenerateJobId() 
        {
            return "";
        }

        public static string GenerateUserID() 
        {
            return Guid.NewGuid().ToString();
        }
    }
}