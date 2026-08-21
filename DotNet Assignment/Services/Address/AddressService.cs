using DotNet_Assignment.Constants;
using DotNet_Assignment.Data;
using DotNet_Assignment.Exceptions;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.Address;
using DotNet_Assignment.Repository.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNet_Assignment.Services.Address
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _context;

        public AddressService(
           IAddressRepository addressRepository,
           IUserRepository userRepository,
           AppDbContext appDbContext)
        {
            _addressRepository = addressRepository;
            _userRepository = userRepository;
            _context = appDbContext;
        }

        /// <summary>
        /// Validates and adds user address for a user
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="addressDto">Address details</param>
        /// <exception cref="Exception">if user not found or user deactivated</exception>
        public async Task AddUserAddressAsync(Guid userId, AddressDto addressDto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new KeyNotFoundException(ExceptionMessages.UserNotFound);
            }

            if (user.IsDeleted)
            {
                throw new UserDeactivatedException(ExceptionMessages.UserDeactivated);
            }

            var address = new UserAddress
            {
                UserId = userId,
                HouseNumber = addressDto.HouseNumber,
                Street = addressDto.Street,
                City = addressDto.City,
                Pincode = addressDto.Pincode,
                Landmark = addressDto.Landmark,
                State = addressDto.State,
            };

            _addressRepository.AddAddress(address);

            await _context.SaveChangesAsync();
        }
        
        /// <summary>
        /// Updates a users address
        /// </summary>
        /// <param name="userid">User ID</param>
        /// <param name="userAddressId">User Address ID</param>
        /// <param name="addressDto">Address details to be updated</param>
        /// <exception cref="Exception">If address not found or user deactivated</exception>
        public async Task UpdateUserAddressAsync(Guid userid, Guid userAddressId, AddressDto addressDto)
        {
            var address = await _addressRepository.GetAddressByIdAsync(userAddressId, userid);

            if (address == null)
            {
                throw new KeyNotFoundException(ExceptionMessages.AddressNotFound);
            }

            if (address.User.IsDeleted)
            {
                throw new UserDeactivatedException(ExceptionMessages.UserDeactivated);
            }

            if (!string.IsNullOrWhiteSpace(addressDto.HouseNumber))
            {
                address.HouseNumber = addressDto.HouseNumber;
            }

            if (!string.IsNullOrWhiteSpace(addressDto.Pincode))
            {
                address.Pincode = addressDto.Pincode;
            }

            if (!string.IsNullOrWhiteSpace(addressDto.Landmark))
            {
                address.Landmark = addressDto.Landmark;
            }

            if (!string.IsNullOrWhiteSpace(addressDto.Street))
            {
                address.Street = addressDto.Street;
            }

            if (!string.IsNullOrWhiteSpace(addressDto.State))
            {
                address.State = addressDto.State;
            }

            if (!string.IsNullOrWhiteSpace(addressDto.City))
            {
                address.City = addressDto.City;
            }

            address.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

        }

    }
}
