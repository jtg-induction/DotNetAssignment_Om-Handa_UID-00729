using DotNet_Assignment.Models.DTO;
using DotNet_Assignment.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Assignment.Services.Users
{
    public interface IUserService
    {
        Task UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto);

        Task DeactivateUserAsync(Guid userId);

        Task ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto);
    }
}
