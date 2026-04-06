using DemoProductsWebAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DemoProductsWebAPI.Application.Interfaces;

namespace DemoProductsWebAPI.Infrastructure.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            // Read-only queries should use AsNoTracking for better performance
            return await _db.Products.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _db.Products.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            await _db.Products.AddAsync(product, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<Product> products, CancellationToken cancellationToken = default)
        {
            await _db.Products.AddRangeAsync(products, cancellationToken);
        }

        public void Update(Product product)
        {
            _db.Products.Update(product);
        }

        public void Remove(Product product)
        {
            _db.Products.Remove(product);
        }
    }
}
