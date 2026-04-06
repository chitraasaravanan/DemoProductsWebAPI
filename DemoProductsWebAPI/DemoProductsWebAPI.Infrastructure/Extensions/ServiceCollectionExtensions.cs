using Microsoft.Extensions.DependencyInjection;
using DemoProductsWebAPI.Application.Interfaces;

namespace DemoProductsWebAPI.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Register DbContext and Infrastructure concrete implementations
            services.AddScoped<IProductRepository, DemoProductsWebAPI.Infrastructure.Data.Repositories.ProductRepository>();
            services.AddScoped<IProductCartRepository, DemoProductsWebAPI.Infrastructure.Data.Repositories.ProductCartRepository>();
            services.AddScoped<IUnitOfWork, DemoProductsWebAPI.Infrastructure.Data.UnitOfWork>();

            // Register Dapper read services and other infra components if needed
            services.AddSingleton<DemoProductsWebAPI.Infrastructure.Data.Read.IDbConnectionFactory, DemoProductsWebAPI.Infrastructure.Data.Read.SqlConnectionFactory>();
            services.AddSingleton<DemoProductsWebAPI.Infrastructure.Data.Read.IDapperExecutor, DemoProductsWebAPI.Infrastructure.Data.Read.DapperExecutor>();

            return services;
        }
    }
}
