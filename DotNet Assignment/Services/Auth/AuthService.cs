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

            string AccessToken = _jWTService.GetAccessToken(user);

            string RefreshToken = _jWTService.GenerateRefreshToken();

            if(RefreshToken == null)
            {
                throw new Exception("Refresh Token Could not be Generated");
            }

            var refresh = new RefreshToken()
            {
                Token = Hasher.Hash(RefreshToken),
                UserId = user.UserId
            };

            _refreshTokenRepository.AddRefreshToken(refresh);

            await _context.SaveChangesAsync();

            return new JWTResponseDto
            {
                AccessToken= AccessToken,
                RefreshToken= RefreshToken,
            };
        }
    }
}
