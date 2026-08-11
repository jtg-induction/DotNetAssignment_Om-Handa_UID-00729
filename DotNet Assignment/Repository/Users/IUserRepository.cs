using DotNet_Assignment.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Assignment.Repository.Users
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<bool> FindUserByEmailAsync(string email);

        void AddUser(User user);
    }

}
