using DotNet_Assignment.Data;
using DotNet_Assignment.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DotNet_Assignment.Repository.Address
{
    public class AddressRepository : IAddressRepository
    {
        private readonly AppDbContext _context;

        public AddressRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserAddress> GetAddressByIdAsync(Guid userAddressId, Guid userId)
        {
            return await _context.UserAddresses.SingleOrDefaultAsync(ua => ua.UserAddressId == userAddressId && ua.UserId == userId);
        }

        public void AddAddress(UserAddress userAddress)
        {
            _context.UserAddresses.Add(userAddress);
        }
    }
}
