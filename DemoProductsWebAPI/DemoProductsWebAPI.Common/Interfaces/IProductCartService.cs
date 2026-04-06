using DemoProductsWebAPI.Application.DTOs;

namespace DemoProductsWebAPI.Common.Interfaces
{
    public interface IProductCartService
    {
        Task<IEnumerable<ProductCartDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProductCartDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ProductCartDto> CreateAsync(ProductCartDto dto, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(ProductCartDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
