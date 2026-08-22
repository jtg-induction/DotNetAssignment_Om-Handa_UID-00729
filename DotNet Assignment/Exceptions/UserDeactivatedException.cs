using DotNet_Assignment.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNet_Assignment.Exceptions
{
    public class UserDeactivatedException :Exception
    {
        public UserDeactivatedException() : base(ExceptionMessages.UserDeactivated) { }

        public UserDeactivatedException(string message) : base(message) { }
    }
}
