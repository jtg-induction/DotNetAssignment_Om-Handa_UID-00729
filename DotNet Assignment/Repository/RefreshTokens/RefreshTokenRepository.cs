using DotNet_Assignment.Data;
using DotNet_Assignment.Models.Entities;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DotNet_Assignment.Utils;

namespace DotNet_Assignment.Repository.RefreshTokens
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Finds if Refresh tokens exists in Database
        /// </summary>
        /// <param name="refreshToken">Unhashed Refresh Token string</param>
        /// <returns>Refresh token from Database</returns>
        public async Task<RefreshToken> GetRefreshTokenAsync(string refreshToken)
        {
            return await _context.RefreshTokens.SingleOrDefaultAsync(u => u.Token == Hasher.Hash(refreshToken));
        }

        /// <summary>
        /// Adds Refresh token to the Database Context
        /// </summary>
        /// <param name="refreshToken">Refresh Token details(token string, id, expiration)</param>
        public void AddRefreshToken(RefreshToken refreshToken)
        {

            _context.RefreshTokens.Add(refreshToken);
        }

        /// <summary>
        /// Deletes a refresh token from DB context
        /// </summary>
        /// <param name="refreshToken">Refresh Token details(token string, id, expiration)</param>
        public void DeleteRefreshToken(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Remove(refreshToken);
        }

    }
}
