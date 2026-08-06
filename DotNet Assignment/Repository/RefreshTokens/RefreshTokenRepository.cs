using DotNet_Assignment.Data;
using DotNet_Assignment.Models.Entities;
using System;
using System.Linq;

namespace DotNet_Assignment.Repository.RefreshTokens
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public RefreshToken GetRefreshToken(string refreshToken)
        {
            return _context.RefreshTokens.SingleOrDefault(u => u.Token == refreshToken);
        }

        public void AddRefreshToken(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
        }

        public void DeleteRefreshToken(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Remove(refreshToken);
        }

        public void DeleteTokensByUserId(Guid userId)
        {
            var Tokens= _context.RefreshTokens.Where(u => u.UserId == userId).ToList();

            _context.RefreshTokens.RemoveRange(Tokens);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

    }
}
