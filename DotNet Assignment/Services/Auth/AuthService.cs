using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Models.Enums;
using DotNet_Assignment.Repository.RefreshTokens;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.JWT;
using DotNet_Assignment.Services.PasswordService;
using System;
using System.Collections.Generic;

namespace DotNet_Assignment.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IJWTService _jWTService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(
            IUserRepository userRepository, 
            IPasswordService passwordService,
            IJWTService jWTService,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _passwordService= passwordService;
            _jWTService= jWTService;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public JWTResponseDto Register(SignupRequestDto requestDto) {

            var ExistingUser = _userRepository.GetUserByEmail(requestDto.Email);

            if (ExistingUser != null) {
                throw new Exception("User Already Exists");
            }

            var User = new User()
            {
                UserId = Guid.NewGuid(),
                Email = requestDto.Email,
                Password = _passwordService.HashPassword(requestDto.Password),
                Name = requestDto.Name,
                PhoneNumber = requestDto.PhoneNumber,
                Role = requestDto.Role,
                IsDeleted = false,
            };

            var Address = new UserAddress()
            {
                UserAddressId = Guid.NewGuid(),
                HouseNumber = requestDto.Address.HouseNumber,
                Street = requestDto.Address.Street,
                Landmark = requestDto.Address.Landmark,
                City = requestDto.Address.City,
                State = requestDto.Address.State,
                Pincode = requestDto.Address.Pincode,
                User = User
            };

            User.UserAddresses = new List<UserAddress>{ Address };

            _userRepository.AddUser(User);

            try
            {
                _userRepository.Save();
            }
            catch(Exception ex)
            {
                throw;
            }

            string AccessToken = _jWTService.GetAccessToken(User);

            string RefreshToken = _jWTService.GetRefreshToken();

            var Refresh = new RefreshToken()
            {
                RefreshTokenId = Guid.NewGuid(),
                Token = RefreshToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = User.UserId
            };

            _refreshTokenRepository.AddRefreshToken(Refresh);

            _refreshTokenRepository.Save();

            return new JWTResponseDto
            {
                AccessToken= AccessToken,
                RefreshToken= RefreshToken,
            };
        }

        public JWTResponseDto Login(LoginRequestDto loginRequest)
        {
            var User = _userRepository.GetUserByEmail(loginRequest.Email);

            if (User == null) {
                throw new Exception("Invalid Credentials");
            }

            if (User.IsDeleted)
            {
                throw new Exception("User Deactivated");
            }

            if (!_passwordService.VerifyPassword(loginRequest.Password, User.Password))
            {
                throw new Exception("Invalid Credentials");
            }

            string AccessToken = _jWTService.GetAccessToken(User);
            string RefreshToken = _jWTService.GetRefreshToken();

            var Refresh = new RefreshToken()
            {
                RefreshTokenId = Guid.NewGuid(),
                Token = RefreshToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = User.UserId
            };

            _refreshTokenRepository.AddRefreshToken(Refresh);

            _refreshTokenRepository.Save();

            return new JWTResponseDto
            {
                AccessToken = AccessToken,
                RefreshToken = RefreshToken,
            };
        }

        public void Logout(LogoutRequestDto logoutRequest)
        {
            var RefreshToken = _refreshTokenRepository.GetRefreshToken(logoutRequest.RefreshToken);

            if (RefreshToken == null) {
                return;
            }

            _refreshTokenRepository.DeleteRefreshToken(RefreshToken);
            _refreshTokenRepository.Save();
        }


        public JWTResponseDto RefreshAccessToken(string refreshToken)
        {
            var Token = _refreshTokenRepository.GetRefreshToken(refreshToken);

            if (Token == null)
            {
                throw new Exception("Invalid Refresh Token");
            }

            if (Token.ExpiresAt < DateTime.UtcNow) 
            {
                throw new Exception("Refresh Token Expired");
            }

            if (Token.User.IsDeleted)
            {
                throw new Exception("User Account is Deactivated");
            }

            var AccessToken = _jWTService.GetAccessToken(Token.User);

            return new JWTResponseDto { 
                AccessToken= AccessToken,
                RefreshToken= refreshToken
            };
        }
    }
}
