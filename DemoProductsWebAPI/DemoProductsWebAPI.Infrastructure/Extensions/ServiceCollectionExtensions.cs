using DemoProductsWebAPI.Common.Interfaces;
using DemoProductsWebAPI.Infrastructure.Data;
using DemoProductsWebAPI.Infrastructure.Data.DapperRepositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DemoProductsWebAPI.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Register Unit of Work (depends on ApplicationDbContext which is already registered)
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register Dapper-based read services
            services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
            services.AddSingleton<IDapperExecutor, DapperExecutor>();

            // Register the read service (for read-optimized queries)
            services.AddScoped<IProductReadRepository, ProductReadRepository>();

            return services;
        }
    }
}
