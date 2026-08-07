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

        public async Task UpdateUserAddressAsync(Guid userid, Guid userAddressId, UpdateAddressDto updateAddressDto)
        {
            var Address = await _userRepository.GetAddressByIdAsync(userAddressId, userid);

            if (Address == null)
            {
                throw new Exception("Address not Found");
            }

            if (Address.User.IsDeleted) {
                throw new Exception("User is Deactivated");
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