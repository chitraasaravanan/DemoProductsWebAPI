using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DemoProductsWebAPI.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProductReadServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register connection factory
            services.AddSingleton<Infrastructure.Data.Read.IDbConnectionFactory, Infrastructure.Data.Read.SqlConnectionFactory>();

            // Dapper executor
            services.AddSingleton<Infrastructure.Data.Read.IDapperExecutor, Infrastructure.Data.Read.DapperExecutor>();

            var redisConn = configuration.GetConnectionString("RedisConnection");
            if (!string.IsNullOrEmpty(redisConn))
            {
                services.AddStackExchangeRedisCache(opts => opts.Configuration = redisConn);
            }

            // Register the infrastructure read service directly. Use output caching in the API layer for response caching.
            services.AddScoped<Application.Interfaces.IProductReadService, Infrastructure.Data.Read.ProductReadService>();

            return services;
        }
    }
}
