using DotNet_Assignment.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNet_Assignment.Exceptions
{
    public class WrongOperationException :Exception
    {
        public WrongOperationException() : base(ExceptionMessages.InvalidOperation) { }

        public WrongOperationException(string message) : base(message) { }
    }
}
