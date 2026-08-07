using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNet_Assignment.Utils
{
    public static class Hasher
    {
        public static string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool Verify(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}