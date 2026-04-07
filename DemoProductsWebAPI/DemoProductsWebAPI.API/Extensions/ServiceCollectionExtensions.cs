using DemoProductsWebAPI.Common.Interfaces;
using DemoProductsWebAPI.Infrastructure.Data.DapperRepositories;

namespace DemoProductsWebAPI.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProductReadServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register connection factory
            services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();

            // Dapper executor
            services.AddSingleton<IDapperExecutor, DapperExecutor>();

            var redisConn = configuration.GetConnectionString("RedisConnection");
            if (!string.IsNullOrEmpty(redisConn))
            {
                services.AddStackExchangeRedisCache(opts => opts.Configuration = redisConn);
            }

            // Register the infrastructure read service directly. Use output caching in the API layer for response caching.
            services.AddScoped<IProductReadRepository, ProductReadRepository>();

            return services;
        }
    }
}
