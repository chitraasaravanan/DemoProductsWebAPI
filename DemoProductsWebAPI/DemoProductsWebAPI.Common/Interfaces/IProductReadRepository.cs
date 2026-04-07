using DemoProductsWebAPI.Common.DTOs;

namespace DemoProductsWebAPI.Common.Interfaces
{
    public interface IProductReadRepository
    {
        Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
