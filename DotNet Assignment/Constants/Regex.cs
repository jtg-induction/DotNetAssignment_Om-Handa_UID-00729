using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNet_Assignment.Constants
{
    public class Regex
    {
        public const string EmailRegex = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";
        public const string PhoneNumberRegex = "^[0-9]{10}$";
        public const string PincodeRegex = "^[1-9][0-9]{5}$";

    }
}