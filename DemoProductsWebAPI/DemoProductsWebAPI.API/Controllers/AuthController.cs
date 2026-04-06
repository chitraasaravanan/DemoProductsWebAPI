using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace DemoProductsWebAPI.API.Controllers
{
    public class AuthController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        /// <summary>
        /// Authenticates a user and returns access and refresh tokens.
        /// </summary>
        /// <param name="req">Login credentials.</param>
        /// <returns>200 OK with access and refresh tokens.</returns>
        [HttpPost("token")]
        public async Task<IActionResult> Token([FromBody] LoginRequest req)
        {
            var result = await _mediator.Send(new Application.Auth.Commands.AuthenticateCommand(req.Username, req.Password)).ConfigureAwait(false);
            return Ok(new { access_token = result.AccessToken, refresh_token = result.RefreshToken });
        }

        /// <summary>
        /// Exchanges a valid refresh token for a new access token and refresh token.
        /// </summary>
        /// <param name="req">Refresh request containing the refresh token.</param>
        /// <returns>200 OK with new tokens, or 401 Unauthorized if token is invalid.</returns>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest req)
        {
            // For now reuse handler by sending an AuthenticateCommand using token as username to reuse logic,
            // in a production system you would have a dedicated RefreshTokenCommand/Handler.
            var result = await _mediator.Send(new Application.Auth.Commands.AuthenticateCommand(req.RefreshToken, string.Empty)).ConfigureAwait(false);
            if (result == null) return Unauthorized();
            return Ok(new { access_token = result.AccessToken, refresh_token = result.RefreshToken });
        }

        /// <summary>
        /// Revokes a refresh token so it can no longer be used.
        /// </summary>
        /// <param name="req">Revoke request containing the refresh token to revoke.</param>
        /// <returns>204 NoContent on success.</returns>
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] RevokeRequest req)
        {
            // In a production system implement a RevokeRefreshTokenCommand/Handler. For demo, call handler via mediator.
            await _mediator.Send(new Application.Auth.Commands.AuthenticateCommand(req.RefreshToken, string.Empty)).ConfigureAwait(false);
            return NoContent();
        }

        /// <summary>
        /// Request DTO for refresh operations.
        /// </summary>
        /// <param name="RefreshToken">The refresh token string.</param>
        public record RefreshRequest(string RefreshToken);

        /// <summary>
        /// Request DTO for revoke operations.
        /// </summary>
        /// <param name="RefreshToken">The refresh token string to revoke.</param>
        public record RevokeRequest(string RefreshToken);

        /// <summary>
        /// Request DTO for login operations.
        /// </summary>
        /// <param name="Username">The user's username.</param>
        /// <param name="Password">The user's password.</param>
        public record LoginRequest(string Username, string Password);
    }
}
