using DemoProductsWebAPI.Domain.Entities;
namespace DemoProductsWebAPI.Common.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default);
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
        Task UpdateAsync(RefreshToken token, CancellationToken cancellationToken = default);
        Task RevokeAsync(string token, CancellationToken cancellationToken = default);
    }
}
