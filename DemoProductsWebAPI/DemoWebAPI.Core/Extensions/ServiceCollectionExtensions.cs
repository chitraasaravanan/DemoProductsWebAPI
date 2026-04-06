using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http;
using System;
using DemoWebAPI.Core.Http;

namespace DemoWebAPI.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommonResilience(this IServiceCollection services, IConfiguration configuration)
        {
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(new[] {
                    TimeSpan.FromMilliseconds(200),
                    TimeSpan.FromMilliseconds(500),
                    TimeSpan.FromSeconds(1)
                });

            var circuitBreaker = HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));

            var wrap = Policy.WrapAsync(retryPolicy, circuitBreaker);
            services.AddSingleton<IAsyncPolicy<HttpResponseMessage>>(wrap);
            services.AddTransient<PolicyDelegatingHandler>();

            return services;
        }
    }
}
