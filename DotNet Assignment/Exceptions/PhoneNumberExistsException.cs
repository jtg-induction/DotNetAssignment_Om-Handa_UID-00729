using DotNet_Assignment.Constants;
using System;

namespace DotNet_Assignment.Exceptions
{
    public class PhoneNumberExistsException : Exception
    {
        public PhoneNumberExistsException() : base(ExceptionMessages.EmailAlreadyExists) { }

        public PhoneNumberExistsException(string message) : base(message) { }
    }
}
