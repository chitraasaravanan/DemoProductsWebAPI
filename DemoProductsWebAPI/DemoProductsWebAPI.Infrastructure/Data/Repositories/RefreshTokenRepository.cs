using DemoProductsWebAPI.Common.Interfaces;
using DemoProductsWebAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoProductsWebAPI.Infrastructure.Data.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _db;

        public RefreshTokenRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default)
        {
            await _db.RefreshTokens.AddAsync(token, cancellationToken);
        }
        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _db.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(r => r.Token == token, cancellationToken);
        }
        public async Task UpdateAsync(RefreshToken token, CancellationToken cancellationToken = default)
        {
            _db.RefreshTokens.Update(token);
            await Task.CompletedTask;
        }

        public async Task RevokeAsync(string token, CancellationToken cancellationToken = default)
        {
            var existing = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token, cancellationToken);
            if (existing != null)
            {
                existing.IsRevoked = true;
            }
        }
    }
}
