using DotNet_Assignment.Data;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using DotNet_Assignment.Repository.Address;
using DotNet_Assignment.Repository.RefreshTokens;
using DotNet_Assignment.Repository.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DotNet_Assignment.Services.Address
{
    public class AddressService :IAddressService
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

        public async Task AddUserAddressAsync(Guid userId, AddressDto addressDto)
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

        public async Task UpdateUserAddressAsync(Guid userid, Guid userAddressId, AddressDto addressDto)
        {
            var address = await _addressRepository.GetAddressByIdAsync(userAddressId, userid);

            if (address == null)
            {
                throw new Exception("Address not Found");
            }

            if (address.User.IsDeleted)
            {
                throw new Exception("User is Deactivated");
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

            await _context.SaveChangesAsync();

        }

    }
}
