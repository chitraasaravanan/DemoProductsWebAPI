using DemoProductsWebAPI.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DemoProductsWebAPI.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register application-level services that may depend on Infrastructure via project reference
            services.AddScoped<IProductService, Services.ProductService>();
            services.AddScoped<IProductCartService, Services.ProductCartService>();

            return services;
        }
    }
}
