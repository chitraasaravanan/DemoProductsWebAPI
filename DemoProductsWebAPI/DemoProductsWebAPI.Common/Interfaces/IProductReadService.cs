using DemoProductsWebAPI.Application.DTOs;

namespace DemoProductsWebAPI.Common.Interfaces
{
    public interface IProductReadService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
