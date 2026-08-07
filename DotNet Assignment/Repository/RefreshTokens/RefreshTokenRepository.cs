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

        public async Task<RefreshToken> GetRefreshTokenAsync(string refreshToken)
        {
            return await _context.RefreshTokens.SingleOrDefaultAsync(u => u.Token == Hasher.Hash(refreshToken));
        }

        public void AddRefreshToken(RefreshToken refreshToken)
        {

            _context.RefreshTokens.Add(refreshToken);
        }

        public void DeleteRefreshToken(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Remove(refreshToken);
        }

    }
}
