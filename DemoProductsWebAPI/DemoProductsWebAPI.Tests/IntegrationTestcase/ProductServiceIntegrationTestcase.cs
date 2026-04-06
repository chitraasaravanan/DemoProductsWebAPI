using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Net.Http;
using System.Net;

namespace DemoProductsWebAPI.Tests.IntegrationTestcase
{
    public class ProductServiceIntegrationTestcase : IClassFixture<TestWebApplicationFactory>
    {
        private readonly TestWebApplicationFactory _factory;

        public ProductServiceIntegrationTestcase(TestWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetProductsEndpoint_Returns_OK()
        {
            var client = _factory.CreateClient();
            var resp = await client.GetAsync("/api/v1.0/products");
            resp.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
