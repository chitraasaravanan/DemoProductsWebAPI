using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using System;

namespace DemoWebAPI.Core.Http
{
    public class PolicyDelegatingHandler : DelegatingHandler
    {
        private readonly IAsyncPolicy<HttpResponseMessage> _policy;

        public PolicyDelegatingHandler(IAsyncPolicy<HttpResponseMessage> policy)
        {
            _policy = policy ?? throw new ArgumentNullException(nameof(policy));
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _policy.ExecuteAsync((ct) => base.SendAsync(request, ct), cancellationToken);
        }
    }
}
