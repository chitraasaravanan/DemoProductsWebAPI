using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;

namespace DemoWebAPI.Core.Policies
{
    public static class PolicyFactory
    {
        public static IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromMilliseconds(200),
                    TimeSpan.FromMilliseconds(500),
                    TimeSpan.FromSeconds(1)
                });
        }

        public static IAsyncPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));
        }

        public static IAsyncPolicy<HttpResponseMessage> CreateWrappedPolicy()
        {
            var retry = CreateRetryPolicy();
            var cb = CreateCircuitBreakerPolicy();
            return Policy.WrapAsync(retry, cb);
        }
    }
}
