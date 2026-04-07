using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace DemoProductsWebAPI.Tests
{
    /// <summary>
    /// Test authentication handler that bypasses JWT validation for integration tests.
    /// Always returns a successfully authenticated principal with basic test claims.
    /// Used in <see cref="TestWebApplicationFactory"/> to allow integration tests to make authenticated requests
    /// without requiring valid JWT tokens.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="TestAuthHandler"/> class.
    /// </remarks>
    /// <param name="options">The authentication scheme options</param>
    /// <param name="logger">The logger factory for diagnostic information</param>
    /// <param name="encoder">The URL encoder for encoding cookie values</param>
    public class TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {

        /// <summary>
        /// Handles the authentication request by creating a test principal with basic claims.
        /// Called by the authentication middleware to authenticate each request.
        /// Flow: Create claims for test user → Build identity → Create principal → Return success result.
        /// </summary>
        /// <returns>AuthenticateResult with test user principal that always succeeds</returns>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Create claims array with basic test user information
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user"),
                new Claim(ClaimTypes.Name, "Test User")
            };

            // Build a claims identity from the test claims
            var identity = new ClaimsIdentity(claims, Scheme.Name);

            // Create a principal from the identity
            var principal = new ClaimsPrincipal(identity);

            // Create authentication ticket with the principal
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            // Return successful authentication result
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
