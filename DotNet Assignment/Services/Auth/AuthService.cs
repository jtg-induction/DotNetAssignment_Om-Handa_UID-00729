using DotNet_Assignment.Constants;
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

        /// <summary>
        /// Registers a user, hashes its password and generated JWT Response
        /// </summary>
        /// <param name="requestDto">Signup details of user</param>
        /// <returns><see cref="JWTResponseDto"/> containing Access and Refresh Token</returns>
        /// <exception cref="Exception">Throws if user already exists or Refresh token was not generated</exception>
        public async Task<JWTResponseDto> RegisterAsync(SignupRequestDto requestDto) {

            if (await _userRepository.FindUserByEmailAsync(requestDto.Email)){
                throw new Exception(ExceptionMessages.EmailAlreadyExists);
            }

            var user = new User()
            {
                Email = requestDto.Email,
                Password = Hasher.Hash(requestDto.Password),
                Name = requestDto.Name,
                PhoneNumber = requestDto.PhoneNumber,
                Role = requestDto.Role,
                IsDeleted = false,
            };

            _userRepository.AddUser(user);

            string accessToken = _jWTService.GetAccessToken(user);

            string refreshToken = _jWTService.GenerateRefreshToken();

            if(refreshToken == null)
            {
                throw new Exception(ExceptionMessages.RefreshTokenNotGenerated);
            }

            var refresh = new RefreshToken()
            {
                Token = Hasher.Hash(refreshToken),
                UserId = user.UserId,
            };

            _refreshTokenRepository.AddRefreshToken(refresh);

            await _context.SaveChangesAsync();

            return new JWTResponseDto
            {
                AccessToken= accessToken,
                RefreshToken= refreshToken,
            };
        }

        /// <summary>
        /// validates and logs a user in
        /// </summary>
        /// <param name="loginRequest">Email and password of a user</param>
        /// <exception cref="Exception">If user not found, user deactivated or wrong password</exception>
        public async Task<JWTResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginRequest.Email);

            if (user == null) {
                throw new Exception(ExceptionMessages.InvalidCredentials);
            }

            if (user.IsDeleted)
            {
                throw new Exception(ExceptionMessages.UserDeactivated);
            }

            if (!Hasher.Verify(loginRequest.Password, user.Password))
            {
                throw new Exception(ExceptionMessages.InvalidCredentials);
            }

            string accessToken = _jWTService.GetAccessToken(user);
            string refreshToken = _jWTService.GenerateRefreshToken();

            var refresh = new RefreshToken()
            {
                Token = _refreshTokenRepository.HashRefreshToken(refreshToken),
                UserId = user.UserId
            };

            _refreshTokenRepository.AddRefreshToken(refresh);

            _context.SaveChanges();

            return new JWTResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
        }

        /// <summary>
        /// Validates and logs out a user
        /// </summary>
        /// <param name="logoutRequest">Refresh token to be removed</param>
        /// <exception cref="Exception">If refresh token is null</exception>
        public async Task LogoutAsync(LogoutRequestDto logoutRequest)
        {
            var hashedRefreshedToken = _refreshTokenRepository.HashRefreshToken(logoutRequest.RefreshToken);
            var refreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(hashedRefreshedToken);

            if (refreshToken == null) {
                throw new Exception(ExceptionMessages.RefreshTokenNotFound);
            }

            _refreshTokenRepository.DeleteRefreshToken(refreshToken);
            _context.SaveChanges();
        }

        /// <summary>
        /// Generates a new access Token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception">If refresh token is invalid or expired or user is deactivated</exception>
        public async Task<JWTResponseDto> RefreshAccessTokenAsync(string refreshToken)
        {
            var hashedToken = _refreshTokenRepository.HashRefreshToken(refreshToken);

            var token = await _refreshTokenRepository.GetRefreshTokenAsync(hashedToken);

            if (token == null)
            { 
                throw new Exception(ExceptionMessages.RefreshTokenInvalid);
            }

            if (token.ExpiresAt < DateTime.UtcNow) 
            {
                throw new Exception(ExceptionMessages.RefreshTokenExpired);
            }

            if (token.User.IsDeleted)
            {
                throw new Exception(ExceptionMessages.UserDeactivated);
            }

            var accessToken = _jWTService.GetAccessToken(token.User);

            return new JWTResponseDto { 
                AccessToken= accessToken,
                RefreshToken= refreshToken
            };
        }
    }
}
