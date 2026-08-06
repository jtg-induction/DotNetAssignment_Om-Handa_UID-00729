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
        void UpdateUser(Guid userId, UpdateUserDto updateUserDto);

        void UpdateUserAddress(Guid userid, Guid userAddressId, UpdateAddressDto updateAddressDto);

        void DeactivateUser(Guid userId);
    }
}
