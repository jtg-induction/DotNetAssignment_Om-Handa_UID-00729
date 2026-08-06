using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.RefreshTokens;
using DotNet_Assignment.Repository.Users;
using DotNet_Assignment.Services.JWT;
using DotNet_Assignment.Services.PasswordService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNet_Assignment.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public UserService(
           IUserRepository userRepository,
           IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public void UpdateUser(Guid userId, UpdateUserDto updateUserDto)
        {
            var User = _userRepository.GetUserById(userId);

            if (User == null)
            {
                throw new Exception("User not found");
            }

            if (!string.IsNullOrWhiteSpace(updateUserDto.Name))
            {
                User.Name = updateUserDto.Name;
            }

            if (!string.IsNullOrWhiteSpace(updateUserDto.PhoneNumber))
            {
                User.PhoneNumber = updateUserDto.PhoneNumber;
            }

            _userRepository.Save();
        }

        public void UpdateUserAddress(Guid userid, Guid userAddressId, UpdateAddressDto updateAddressDto)
        {
            var Address = _userRepository.GetAddressById(userAddressId, userid);

            if (Address == null)
            {
                throw new Exception("Address not Found");
            }

            if (!string.IsNullOrWhiteSpace(updateAddressDto.HouseNumber))
            {
                Address.HouseNumber = updateAddressDto.HouseNumber;
            }

            if (!string.IsNullOrWhiteSpace(updateAddressDto.Pincode))
            {
                Address.Pincode = updateAddressDto.Pincode;
            }

            if (!string.IsNullOrWhiteSpace(updateAddressDto.Landmark))
            {
                Address.Landmark = updateAddressDto.Landmark;
            }

            if (!string.IsNullOrWhiteSpace(updateAddressDto.Street))
            {
                Address.Street = updateAddressDto.Street;
            }

            if (!string.IsNullOrWhiteSpace(updateAddressDto.State))
            {
                Address.State = updateAddressDto.State;
            }

            if (!string.IsNullOrWhiteSpace(updateAddressDto.City))
            {
                Address.City = updateAddressDto.City;
            }

            _userRepository.Save();

        }

        public void DeactivateUser(Guid userId) {
            var User = _userRepository.GetUserById(userId);

            if (User == null)
            {
                throw new Exception("User not found");
            }

            User.IsDeleted = true;

            _refreshTokenRepository.DeleteTokensByUserId(userId);

            _userRepository.Save();
            _refreshTokenRepository.Save();
          
        }
    }
}