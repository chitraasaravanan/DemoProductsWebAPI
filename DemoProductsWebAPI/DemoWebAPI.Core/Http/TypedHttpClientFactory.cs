using Microsoft.Extensions.DependencyInjection;

namespace DemoWebAPI.Core.Http
{
    public static class TypedHttpClientFactory
    {
        public static void RegisterTypedClient(IServiceCollection services, string name, string? baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl)) return;
            services.AddHttpClient(name, client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = TimeSpan.FromSeconds(10);
            })
            .AddHttpMessageHandler<PolicyDelegatingHandler>();
        }
    }
}
