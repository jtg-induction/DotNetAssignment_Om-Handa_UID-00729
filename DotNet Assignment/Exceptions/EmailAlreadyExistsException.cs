using DotNet_Assignment.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNet_Assignment.Exceptions
{
    public class EmailAlreadyExistsException : Exception
    {
        public EmailAlreadyExistsException() : base(ExceptionMessages.EmailAlreadyExists) { }

        public EmailAlreadyExistsException(string message) : base(message) { }
    }
}
