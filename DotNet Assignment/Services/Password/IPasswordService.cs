using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Assignment.Services.PasswordService
{
    public interface IPasswordService
    {
        string HashPassword(string password);

        bool VerifyPassword(string password, string hash);
    }
}
