using DemoProductsWebAPI.Domain.Entities;

namespace DemoProductsWebAPI.Common.Interfaces
{
    public interface IProductCartRepository
    {
        Task<List<ProductCart>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProductCart?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task AddAsync(ProductCart item, CancellationToken cancellationToken = default);
        void Update(ProductCart item);
        void Remove(ProductCart item);
    }
}
