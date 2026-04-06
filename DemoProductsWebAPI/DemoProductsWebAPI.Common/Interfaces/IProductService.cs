using DemoProductsWebAPI.Application.DTOs;

namespace DemoProductsWebAPI.Common.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ProductDto> CreateAsync(ProductDto dto, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductDto>> BulkInsertAsync(IEnumerable<ProductDto> dtos, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(ProductDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
