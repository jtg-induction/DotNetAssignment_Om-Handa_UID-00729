using DotNet_Assignment.Data;
using DotNet_Assignment.Models.Entities;
using System.Linq;

namespace DotNet_Assignment.Repository.Users
{
    public class UserRepository : IUserRepository
    {

        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.SingleOrDefault(u => u.Email == email);
        }

        public void AddUser(User user) { 
            _context.Users.Add(user);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
