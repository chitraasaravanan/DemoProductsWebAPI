using DemoProductsWebAPI.Domain.Entities;

namespace DemoProductsWebAPI.Common.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task AddAsync(Product product, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<Product> products, CancellationToken cancellationToken = default);
        void Update(Product product);
        void Remove(Product product);
    }
}
