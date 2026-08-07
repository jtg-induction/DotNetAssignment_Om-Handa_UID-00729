using DotNet_Assignment.Data;
using DotNet_Assignment.Models.Entities;
using System;
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

        public User GetUserById(Guid userId)
        {
            return _context.Users.SingleOrDefault(u => u.UserId == userId);
        }

        public UserAddress GetAddressById(Guid userAddressId, Guid userId)
        {
            return _context.UserAddresses.SingleOrDefault(ua=> ua.UserAddressId== userAddressId && ua.UserId==userId);
        }

        public void AddUser(User user) { 
            _context.Users.Add(user);
        }
    }
}
