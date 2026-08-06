using DotNet_Assignment.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Assignment.Services.Auth
{
    public interface IAuthService
    {
        JWTResponseDto Register(SignupRequestDto requestDto);

        JWTResponseDto Login(LoginRequestDto loginRequest);

        void Logout(LogoutRequestDto logoutRequest);

        JWTResponseDto RefreshAccessToken(string refreshToken);
    }
}
