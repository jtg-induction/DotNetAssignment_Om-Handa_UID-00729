using DotNet_Assignment.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Assignment.Tests.Utils
{
    public static class AuthTestUtil
    {
        public static SignupRequestDto CreateMockSignupRequestDto()
        {
            return new SignupRequestDto
            {
                Name = "Om Handa",
                Email = "omhanda@test.com",
                Password = "12345678",
                PhoneNumber = "1234567890",
            };
        }

        public static LoginRequestDto CreateMockLoginRequestDto()
        {
            return new LoginRequestDto
            {
                Email = "omhanda@test.com",
                Password = "12345678"
            };
        }

        public static LogoutRequestDto CreateMockLogoutRequestDto()
        {
            return new LogoutRequestDto
            {
                RefreshToken= "MockRefreshToken"
            };
        }
    }
}
