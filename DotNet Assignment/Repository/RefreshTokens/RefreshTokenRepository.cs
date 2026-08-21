using DotNet_Assignment.Data;
using DotNet_Assignment.Models.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using DotNet_Assignment.Utils;
using System.Data.Entity;
using System.Security.Cryptography;
using System.Text;

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
            return await _context.RefreshTokens.SingleOrDefaultAsync(u => u.Token == refreshToken);
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

        /// <summary>
        /// Deletes all refresh token of a specific user
        /// </summary>
        /// <param name="userId">User id to find tokens</param>
        public void DeleteTokensByUserId(Guid userId)
        {
            var tokens= _context.RefreshTokens.Where(u => u.UserId == userId).ToList();

            _context.RefreshTokens.RemoveRange(tokens);
        }

        /// <summary>
        /// Hashes a refresh token using SHA256
        /// </summary>
        /// <param name="refreshToken">Unhashed Refresh token string</param>
        /// <returns>Hashed refresh Token String</returns>
        public string HashRefreshToken(string refreshToken)
        {
            using(var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(refreshToken);

                var hash = sha256.ComputeHash(bytes);

                return Convert.ToBase64String(hash);
            }
        }

    }
}
