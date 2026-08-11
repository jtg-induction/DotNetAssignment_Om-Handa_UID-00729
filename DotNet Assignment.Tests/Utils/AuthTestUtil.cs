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
        /// <summary>
        /// Creates a mock signup request DTO
        /// </summary>
        /// <returns>Signup Request Dto</returns>
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

        /// <summary>
        /// Creates a mock login request DTO
        /// </summary>
        /// <returns>Login Request Dto</returns>
        public static LoginRequestDto CreateMockLoginRequestDto()
        {
            return new LoginRequestDto
            {
                Email = "omhanda@test.com",
                Password = "12345678"
            };
        }

        /// <summary>
        /// Creates a mock logout request DTO
        /// </summary>
        /// <returns>Logout Request Dto</returns>
        public static LogoutRequestDto CreateMockLogoutRequestDto()
        {
            return new LogoutRequestDto
            {
                RefreshToken= "MockRefreshToken"
            };
        }
    }
}
