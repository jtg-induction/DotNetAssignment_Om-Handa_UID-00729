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

        /// <summary>
        /// Finds if user exists in DB based on Email and returns it
        /// </summary>
        /// <param name="email">User Email</param>
        /// <returns>User details if found else Null</returns>
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Checks if user exists in DB based on email
        /// </summary>
        /// <param name="email">User Email</param>
        /// <returns>True if user Exists, false if does not</returns>
        public async Task<bool> FindUserByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId);
        }

        /// <summary>
        /// Adds a user to the Database context
        /// </summary>
        /// <param name="user">User details</param>
        public void AddUser(User user) { 
            _context.Users.Add(user);
        }
    }
}
