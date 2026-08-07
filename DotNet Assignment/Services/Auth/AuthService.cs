using DotNet_Assignment.Data;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Models.Enums;
using DotNet_Assignment.Repository.RefreshTokens;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.JWT;
using DotNet_Assignment.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNet_Assignment.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJWTService _jWTService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly AppDbContext _context;

        public AuthService(
            IUserRepository userRepository, 
            IJWTService jWTService,
            IRefreshTokenRepository refreshTokenRepository,
            AppDbContext appDbContext)
        {
            _userRepository = userRepository;
            _jWTService= jWTService;
            _refreshTokenRepository = refreshTokenRepository;
            _context = appDbContext;
        }

        public async Task<JWTResponseDto> RegisterAsync(SignupRequestDto requestDto) {

            if (await _userRepository.FindUserByEmailAsync(requestDto.Email)){
                throw new Exception("Email Already Exists");
            }

            var User = new User()
            {
                Email = requestDto.Email,
                Password = Hasher.Hash(requestDto.Password),
                Name = requestDto.Name,
                PhoneNumber = requestDto.PhoneNumber,
                Role = requestDto.Role,
                IsDeleted = false,
            };

            _userRepository.AddUser(User);

            string AccessToken = _jWTService.GetAccessToken(User);

            string RefreshToken = _jWTService.GenerateRefreshToken();

            if(RefreshToken == null)
            {
                throw new Exception("Refresh Token Could not be Generated");
            }

            var Refresh = new RefreshToken()
            {
                Token = Hasher.Hash(RefreshToken),
                UserId = User.UserId
            };

            _refreshTokenRepository.AddRefreshToken(Refresh);

            try
            {
            await _context.SaveChangesAsync();

            }
            catch(Exception ex)
            {
                throw;
            }

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
