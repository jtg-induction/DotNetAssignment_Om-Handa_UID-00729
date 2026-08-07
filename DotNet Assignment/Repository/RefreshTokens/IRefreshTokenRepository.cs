using DotNet_Assignment.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Assignment.Repository.RefreshTokens
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetRefreshTokenAsync(string refreshToken);

        void AddRefreshToken(RefreshToken refreshToken);

        void DeleteRefreshToken(RefreshToken refreshToken);

        void DeleteTokensByUserId(Guid userId);

    }
}
