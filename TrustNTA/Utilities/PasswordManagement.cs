using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BCrypt.Net;

namespace TrustNTA.Utilities
{
    public static  class PasswordManagement
    {
        public static string HashPassword(string input) 
        {
            return BCrypt.Net.BCrypt.HashPassword(input);
        }

        public static bool VerifyPassword(string password, string hash) 
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}