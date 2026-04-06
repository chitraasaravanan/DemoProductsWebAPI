using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using DemoProductsWebAPI.Common.Interfaces;
using DemoProductsWebAPI.Infrastructure.Data;
using DemoProductsWebAPI.Infrastructure.Data.Read;

namespace DemoProductsWebAPI.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register Unit of Work (depends on ApplicationDbContext which is already registered)
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register Dapper-based read services
            services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
            services.AddSingleton<IDapperExecutor, DapperExecutor>();

            // Register the read service
            services.AddScoped<IProductReadService, ProductReadService>();

            return services;
        }
    }
}
