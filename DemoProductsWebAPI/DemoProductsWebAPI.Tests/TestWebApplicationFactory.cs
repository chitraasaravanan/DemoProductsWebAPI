using DemoProductsWebAPI.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DemoProductsWebAPI.Tests
{
    /// <summary>
    /// Factory for creating test instances of the ASP.NET Core application for integration tests.
    /// Overrides dependency injection to use in-memory database and test authentication instead of production services.
    /// Ensures each test has isolated, clean data and does not require external services.
    /// 
    /// Key configurations:
    /// - Replaces SQL Server DbContext with in-memory EF Core database
    /// - Replaces JWT authentication with <see cref="TestAuthHandler"/> that always succeeds
    /// - Provides clean isolation for each test instance
    /// </summary>
    public class TestWebApplicationFactory : WebApplicationFactory<DemoProductsWebAPI.API.Program>
    {
        /// <summary>
        /// Creates and configures the host for test execution.
        /// Replaces production services with test implementations for database and authentication.
        /// Flow: Remove production DbContext → Add in-memory DbContext → Remove JWT auth → Add test auth.
        /// </summary>
        /// <param name="builder">The host builder to configure</param>
        /// <returns>Configured IHost with test services</returns>
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove production DbContext registrations to avoid conflicts with test configuration
                var toRemove = services.Where(d => (d.ServiceType?.FullName?.Contains("ApplicationDbContext") ?? false) ||
                                                   (d.ImplementationType?.FullName?.Contains("ApplicationDbContext") ?? false) ||
                                                   (d.ServiceType?.FullName?.Contains("IDbContextPool") ?? false) ||
                                                   (d.ImplementationType?.FullName?.Contains("DbContextPool") ?? false)).ToList();

                foreach (var d in toRemove)
                {
                    services.Remove(d);
                }

                // Register in-memory database for fast, isolated test execution
                // Each test gets a unique database instance with same name to test from clean state
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Remove JWT-based authentication services registered in Program.cs
                // This prevents authentication failures in integration tests without valid tokens
                var authServices = services.Where(d => d.ServiceType?.FullName?.Contains("Authentication") ?? false).ToList();
                foreach (var d in authServices)
                {
                    services.Remove(d);
                }

                // Register test authentication that always succeeds
                // Allows integration tests to make authenticated requests without JWT token generation
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "TestScheme";
                    options.DefaultChallengeScheme = "TestScheme";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", null);
            });

            return base.CreateHost(builder);
        }
    }
}
