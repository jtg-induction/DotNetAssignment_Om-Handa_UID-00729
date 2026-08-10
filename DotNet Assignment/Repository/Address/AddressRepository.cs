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

        /// <summary>
        /// Gets User Address from DB context
        /// </summary>
        /// <param name="userAddressId">Users Address ID to search</param>
        /// <param name="userId">user ID</param>
        /// <returns>User Address if Found else null</returns>
        public async Task<UserAddress> GetAddressByIdAsync(Guid userAddressId, Guid userId)
        {
            return await _context.UserAddresses.SingleOrDefaultAsync(ua => ua.UserAddressId == userAddressId && ua.UserId == userId);
        }

        /// <summary>
        /// Adds User address to DB context
        /// </summary>
        /// <param name="userAddress">User address Details</param>
        public void AddAddress(UserAddress userAddress)
        {
            _context.UserAddresses.Add(userAddress);
        }
    }
}
