using DemoProductsWebAPI.Common.Interfaces;
using DemoProductsWebAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoProductsWebAPI.Infrastructure.Data.Repositories
{
    public class ProductCartRepository(ApplicationDbContext db) : IProductCartRepository
    {
        private readonly ApplicationDbContext _db = db;

        public async Task<List<ProductCart>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Set<ProductCart>().AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<ProductCart?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _db.Set<ProductCart>().FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task AddAsync(ProductCart item, CancellationToken cancellationToken = default)
        {
            await _db.Set<ProductCart>().AddAsync(item, cancellationToken);
        }

        public void Update(ProductCart item)
        {
            _db.Set<ProductCart>().Update(item);
        }

        public void Remove(ProductCart item)
        {
            _db.Set<ProductCart>().Remove(item);
        }
    }
}
