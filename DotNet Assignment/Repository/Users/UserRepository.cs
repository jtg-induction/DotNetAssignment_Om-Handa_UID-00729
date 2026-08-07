using DotNet_Assignment.Data;
using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet_Assignment.Repository.Users
{
    public class UserRepository : IUserRepository
    {

        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> FindUserByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<UserAddress> GetAddressByIdAsync(Guid userAddressId, Guid userId)
        {
            return await _context.UserAddresses.SingleOrDefaultAsync(ua=> ua.UserAddressId== userAddressId && ua.UserId==userId);
        }

        public void AddUser(User user) { 
            _context.Users.Add(user);
        }
        public void AddAddress(UserAddress userAddress)
        {
           _context.UserAddresses.Add(userAddress);
        }
    }
}
