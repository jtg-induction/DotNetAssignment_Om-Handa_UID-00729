using DotNet_Assignment.Constants;
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
        
        /// <summary>
        /// Updates user Details
        /// </summary>
        /// <param name="userId">User id to find user</param>
        /// <param name="updateUserDto">User details to be upadated</param>
        /// <exception cref="Exception">If user not found or deactivated</exception>
        public async Task UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new Exception(ExceptionMessages.UserNotFound);
            }

            if (user.IsDeleted)
            {
                throw new Exception(ExceptionMessages.UserDeactivated);
            }

            if(string.IsNullOrWhiteSpace(updateUserDto.Name) && string.IsNullOrWhiteSpace(updateUserDto.PhoneNumber))
            {
                throw new Exception(ExceptionMessages.OneFieldRequired);
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

        /// <summary>
        /// Deactivates a user profile
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <exception cref="Exception">If user not found or user deactivated</exception>
        public async Task DeactivateUserAsync(Guid userId) {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new Exception(ExceptionMessages.UserNotFound);
            }

            if (user.IsDeleted)
            {
                throw new Exception(ExceptionMessages.UserDeactivated);
            }

            user.IsDeleted = true;

            _refreshTokenRepository.DeleteTokensByUserId(userId);

            await _context.SaveChangesAsync();

        }

        /// <summary>
        /// Changes a user's password
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="changePasswordDto">Users old and new password</param>
        /// <exception cref="Exception">If user is null, deactivated or old password is incorrect</exception>
        public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new Exception(ExceptionMessages.UserNotFound);
            }

            if (user.IsDeleted)
            {
                throw new Exception(ExceptionMessages.UserDeactivated);
            }

            if (!Hasher.Verify(changePasswordDto.OldPassword, user.Password))
            {
                throw new Exception(ExceptionMessages.OldPasswordIncorrect);
            }

            user.Password = Hasher.Hash(changePasswordDto.NewPassword);

            await _context.SaveChangesAsync();
        }
    }
}
