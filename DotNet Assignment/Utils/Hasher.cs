using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNet_Assignment.Utils
{
    public static class Hasher
    {
        /// <summary>
        /// Hashes a password using bcrypt
        /// </summary>
        /// <param name="password">user Password</param>
        /// <returns>hashed password</returns>
        public static string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Verifies user password using bcrypt
        /// </summary>
        /// <param name="password">Password user entered</param>
        /// <param name="hash">User Password in DB</param>
        /// <returns>True is verified and false if not verified</returns>
        public static bool Verify(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
