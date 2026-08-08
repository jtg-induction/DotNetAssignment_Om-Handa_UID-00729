using DotNet_Assignment.Data;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.RefreshTokens;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Utils;
using System;
using System.Threading.Tasks;

namespace DotNet_Assignment.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly AppDbContext _context;

        public UserService(
           IUserRepository userRepository,
           IRefreshTokenRepository refreshTokenRepository,
           AppDbContext appDbContext)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _context = appDbContext;
        }

        public async Task UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (user.IsDeleted)
            {
                throw new Exception("User is Deactivated");
            }

            if (!string.IsNullOrWhiteSpace(updateUserDto.Name))
            {
                user.Name = updateUserDto.Name;
            }

            if (!string.IsNullOrWhiteSpace(updateUserDto.PhoneNumber))
            {
                user.PhoneNumber = updateUserDto.PhoneNumber;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeactivateUserAsync(Guid userId) {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (user.IsDeleted)
            {
                throw new Exception("User Already Deactivated");
            }

            user.IsDeleted = true;

            _refreshTokenRepository.DeleteTokensByUserId(userId);

            await _context.SaveChangesAsync();

        }

        public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (user.IsDeleted)
            {
                throw new Exception("User Already Deactivated");
            }

            if(!Hasher.Verify(changePasswordDto.OldPassword, user.Password))
            {
                throw new Exception("Old Password is Incorrect");
            }

            user.Password = Hasher.Hash(changePasswordDto.NewPassword);

            await _context.SaveChangesAsync();
        }
    }
}
