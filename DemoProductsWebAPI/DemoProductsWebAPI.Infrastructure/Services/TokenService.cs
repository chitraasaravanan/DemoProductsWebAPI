using DemoProductsWebAPI.Common.Interfaces;
using DemoWebAPI.Core.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DemoProductsWebAPI.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _settings;
        private readonly byte[] _key;

        public TokenService(Microsoft.Extensions.Options.IOptions<JwtSettings> options)
        {
            _settings = options.Value;
            _key = Encoding.UTF8.GetBytes(_settings.Key ?? string.Empty);
        }

        public string GenerateAccessToken(string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(_key);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId) };
            var token = new JwtSecurityToken(_settings.Issuer, _settings.Audience, claims, expires: DateTime.UtcNow.AddMinutes(_settings.ExpireMinutes), signingCredentials: creds);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public bool ValidateAccessToken(string token, out string? userId)
        {
            userId = null;
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _settings.Issuer,
                    ValidAudience = _settings.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(_key),
                    ValidateIssuerSigningKey = true
                };

                var principal = tokenHandler.ValidateToken(token, parameters, out _);
                userId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
