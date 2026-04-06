using DemoProductsWebAPI.Application.DTOs;

namespace DemoProductsWebAPI.Common.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(string userId);
        string GenerateRefreshToken();
        bool ValidateAccessToken(string token, out string? userId);
    }
}
