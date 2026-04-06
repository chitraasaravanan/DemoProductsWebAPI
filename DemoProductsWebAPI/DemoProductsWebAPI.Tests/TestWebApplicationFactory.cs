using System.Linq;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using DemoProductsWebAPI.Infrastructure.Data;
using Microsoft.Extensions.Hosting;

namespace DemoProductsWebAPI.Tests
{
    public class TestWebApplicationFactory : WebApplicationFactory<DemoProductsWebAPI.API.Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove anything related to ApplicationDbContext to avoid conflicts (DbContextPool, options, etc.)
                var toRemove = services.Where(d => (d.ServiceType?.FullName?.Contains("ApplicationDbContext") ?? false) ||
                                                   (d.ImplementationType?.FullName?.Contains("ApplicationDbContext") ?? false) ||
                                                   (d.ServiceType?.FullName?.Contains("IDbContextPool") ?? false) ||
                                                   (d.ImplementationType?.FullName?.Contains("DbContextPool") ?? false)).ToList();

                foreach (var d in toRemove)
                {
                    services.Remove(d);
                }

                // Register ApplicationDbContext using InMemory for isolation in tests
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });
            });

            return base.CreateHost(builder);
        }
    }
}
