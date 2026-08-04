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
                Id = Guid.NewGuid(),
                Email = requestDto.Email,
                Password = _passwordService.HashPassword(requestDto.Password),
                Name = requestDto.Name,
                PhoneNumber = requestDto.PhoneNumber,
                Balance = 1000m,
                Role = requestDto.Role,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            var Address = new UserAddress()
            {
                Id = Guid.NewGuid(),
                HouseNumber = requestDto.Address.HouseNumber,
                Street = requestDto.Address.Street,
                Landmark = requestDto.Address.Landmark,
                City = requestDto.Address.City,
                State = requestDto.Address.State,
                Pincode = requestDto.Address.Pincode,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                User = User
            };

            User.UserAddresses = new List<UserAddress>{ Address };

            _userRepository.AddUser(User);

            _userRepository.Save();

            string AccessToken = _jWTService.GetAccessToken(User);

            string RefreshToken = _jWTService.GetRefreshToken();

            var Refresh = new RefreshToken()
            {
                RefreshTokenId = Guid.NewGuid(),
                Token = RefreshToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = User.Id
            };

            _refreshTokenRepository.AddRefreshToken(Refresh);

            _refreshTokenRepository.Save();

            return new JWTResponseDto
            {
                AccessToken= AccessToken,
                RefreshToken= RefreshToken,
            };
        }
    }
}
