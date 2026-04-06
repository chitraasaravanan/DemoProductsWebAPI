using DemoProductsWebAPI.Infrastructure.Data.Repositories;
using DemoProductsWebAPI.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DemoProductsWebAPI.Infrastructure.Data
{
    public class UnitOfWork : Application.Interfaces.IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Products = new ProductRepository(_db);
            RefreshTokens = new Repositories.RefreshTokenRepository(_db);
            ProductCarts = new Repositories.ProductCartRepository(_db);
        }

        public IProductRepository Products { get; }
        public Application.Interfaces.IRefreshTokenRepository RefreshTokens { get; }
        public IProductCartRepository ProductCarts { get; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default)
        {
            if (!_db.Database.IsRelational())
            {
                await operation().ConfigureAwait(false);
                return;
            }

            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await operation().ConfigureAwait(false);
                await tx.CommitAsync(cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                await tx.RollbackAsync(cancellationToken).ConfigureAwait(false);
                throw;
            }
        }
    }
}
