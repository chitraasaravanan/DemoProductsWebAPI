using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using DemoProductsWebAPI.Common.Interfaces;

namespace DemoProductsWebAPI.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register application-level services that may depend on Infrastructure via project reference
            services.AddScoped<IProductService, Services.ProductService>();
            services.AddScoped<IProductCartService, Services.ProductCartService>();

            // Register repositories and unit of work by delegating to Infrastructure assembly's factory methods
            // Try to locate an Infrastructure registration extension method and invoke it so API doesn't need a direct compile-time reference.
            try
            {
                var infraAssembly = System.AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "DemoProductsWebAPI.Infrastructure");
                if (infraAssembly != null)
                {
                    var extType = infraAssembly.GetTypes().FirstOrDefault(t => t.Name == "ServiceCollectionExtensions" && t.Namespace == "DemoProductsWebAPI.Infrastructure.Extensions");
                    var method = extType?.GetMethod("AddInfrastructureServices", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null, new[] { typeof(IServiceCollection) }, null);
                    if (method != null)
                    {
                        method.Invoke(null, new object[] { services });
                    }
                }
            }
            catch
            {
                // ignore - registration may be performed by the host
            }
            return services;
        }
    }
}
