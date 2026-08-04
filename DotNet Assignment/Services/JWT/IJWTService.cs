using DotNet_Assignment.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Assignment.Services.JWT
{
    public interface IJWTService
    {
        string GetAccessToken(User user);
        string GetRefreshToken();
    }
}
