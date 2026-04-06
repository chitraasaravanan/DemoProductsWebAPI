using MediatR;
using DemoProductsWebAPI.Application.Auth.Commands;
using DemoProductsWebAPI.Application.DTOs;
using DemoProductsWebAPI.Application.Interfaces;

namespace DemoProductsWebAPI.Application.Auth.Handlers
{
    public class AuthenticateHandler : IRequestHandler<AuthenticateCommand, AuthResultDto>
    {
        private readonly ITokenService _tokens;
        private readonly IUnitOfWork _uow;

        public AuthenticateHandler(ITokenService tokens, IUnitOfWork uow)
        {
            _tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        public async Task<AuthResultDto> Handle(AuthenticateCommand request, CancellationToken cancellationToken)
        {
            // For demo purposes, accept any username/password
            var userId = request.Username;
            var access = _tokens.GenerateAccessToken(userId);
            var refresh = _tokens.GenerateRefreshToken();

            var rt = new DemoProductsWebAPI.Domain.Entities.RefreshToken
            {
                Token = refresh,
                UserId = userId,
                ExpiresOn = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _uow.RefreshTokens.AddAsync(rt);
            await _uow.SaveChangesAsync(cancellationToken);

            return new AuthResultDto { AccessToken = access, RefreshToken = refresh };
        }
    }
}
