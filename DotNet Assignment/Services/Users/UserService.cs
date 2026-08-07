using DotNet_Assignment.Data;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.RefreshTokens;
using DotNet_Assignment.Repository.Users;
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
            var User = await _userRepository.GetUserByIdAsync(userId);

            if (User == null)
            {
                throw new Exception("User not found");
            }

            if (User.IsDeleted)
            {
                throw new Exception("User is Deactivated");
            }

            if (!string.IsNullOrWhiteSpace(updateUserDto.Name))
            {
                User.Name = updateUserDto.Name;
            }

            if (!string.IsNullOrWhiteSpace(updateUserDto.PhoneNumber))
            {
                User.PhoneNumber = updateUserDto.PhoneNumber;
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAddressAsync(Guid userid, Guid userAddressId, AddressDto addressDto)
        {
            var Address = await _userRepository.GetAddressByIdAsync(userAddressId, userid);

            if (Address == null)
            {
                throw new Exception("Address not Found");
            }

            if (Address.User.IsDeleted) {
                throw new Exception("User is Deactivated");
            }

            if (!string.IsNullOrWhiteSpace(addressDto.HouseNumber))
            {
                Address.HouseNumber = addressDto.HouseNumber;
            }

            if (!string.IsNullOrWhiteSpace(addressDto.Pincode))
            {
                Address.Pincode = addressDto.Pincode;
            }

            if (!string.IsNullOrWhiteSpace(addressDto.Landmark))
            {
                Address.Landmark = addressDto.Landmark;
            }

            if (!string.IsNullOrWhiteSpace(addressDto.Street))
            {
                Address.Street = addressDto.Street;
            }

            if (!string.IsNullOrWhiteSpace(addressDto.State))
            {
                Address.State = addressDto.State;
            }

            if (!string.IsNullOrWhiteSpace(addressDto.City))
            {
                Address.City = addressDto.City;
            }

            await _context.SaveChangesAsync();

        }

        public async Task DeactivateUserAsync(Guid userId) {
            var User = await _userRepository.GetUserByIdAsync(userId);

            if (User == null)
            {
                throw new Exception("User not found");
            }

            if (User.IsDeleted)
            {
                throw new Exception("User Already Deactivated");
            }

            User.IsDeleted = true;

            _refreshTokenRepository.DeleteTokensByUserId(userId);

            await _context.SaveChangesAsync();

        }

        public async Task AddUserAddressAsync(Guid userId, AddressDto addressDto)
        {
            var User= await _userRepository.GetUserByIdAsync(userId);


            if (User == null)
            {
                throw new Exception("User not found");
            }

            if (User.IsDeleted)
            {
                throw new Exception("User Already Deactivated");
            }

            var Address = new UserAddress
            {
                UserId = userId,
                HouseNumber = addressDto.HouseNumber,
                Street = addressDto.Street,
                City = addressDto.City,
                Pincode = addressDto.Pincode,
                Landmark = addressDto.Landmark,
                State = addressDto.State,
            };

            _userRepository.AddAddress(Address);

            await _context.SaveChangesAsync();
        }
    }
}