using DemoProductsWebAPI.Common.DTOs;
using FluentAssertions;
using System.Net;
using System.Text;
using System.Text.Json;

namespace DemoProductsWebAPI.Tests.IntegrationTestcase
{
    public class ProductIntegrationTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly TestWebApplicationFactory _factory;

        public ProductIntegrationTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetAllProducts_ReturnsOK_WithEmptyList()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1.0/products");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CreateProduct_ReturnsCreated_WithValidData()
        {
            // Arrange
            var client = _factory.CreateClient();
            var dto = new ProductDto
            {
                ProductName = "Test Product",
                CreatedBy = "integration-test",
                CreatedOn = DateTime.UtcNow
            };
            var content = new StringContent(
                JsonSerializer.Serialize(dto),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await client.PostAsync("/api/v1.0/products", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task GetProductById_ReturnsOK_WhenProductExists()
        {
            // Arrange
            var client = _factory.CreateClient();
            // First create a product
            var createDto = new ProductDto
            {
                ProductName = "Test Product",
                CreatedBy = "integration-test",
                CreatedOn = DateTime.UtcNow
            };
            var createContent = new StringContent(
                JsonSerializer.Serialize(createDto),
                Encoding.UTF8,
                "application/json");
            var createResponse = await client.PostAsync("/api/v1.0/products", createContent);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdContent = await createResponse.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(createdContent);
            var productId = doc.RootElement.GetProperty("id").GetInt32();

            // Act
            var getResponse = await client.GetAsync($"/api/v1.0/products/{productId}");

            // Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1.0/products/99999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var client = _factory.CreateClient();
            // First create a product
            var createDto = new ProductDto
            {
                ProductName = "Original Product",
                CreatedBy = "integration-test",
                CreatedOn = DateTime.UtcNow
            };
            var createContent = new StringContent(
                JsonSerializer.Serialize(createDto),
                Encoding.UTF8,
                "application/json");
            var createResponse = await client.PostAsync("/api/v1.0/products", createContent);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdContent = await createResponse.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(createdContent);
            var productId = doc.RootElement.GetProperty("id").GetInt32();

            // Act
            var updateDto = new ProductDto
            {
                Id = productId,
                ProductName = "Updated Product",
                CreatedBy = "integration-test",
                CreatedOn = DateTime.UtcNow
            };
            var updateContent = new StringContent(
                JsonSerializer.Serialize(updateDto),
                Encoding.UTF8,
                "application/json");
            var updateResponse = await client.PutAsync($"/api/v1.0/products/{productId}", updateContent);

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            var client = _factory.CreateClient();
            var updateDto = new ProductDto
            {
                Id = 1,
                ProductName = "Updated Product",
                CreatedBy = "integration-test",
                CreatedOn = DateTime.UtcNow
            };
            var updateContent = new StringContent(
                JsonSerializer.Serialize(updateDto),
                Encoding.UTF8,
                "application/json");

            // Act
            var updateResponse = await client.PutAsync("/api/v1.0/products/999", updateContent);

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var client = _factory.CreateClient();
            // First create a product
            var createDto = new ProductDto
            {
                ProductName = "Product to Delete",
                CreatedBy = "integration-test",
                CreatedOn = DateTime.UtcNow
            };
            var createContent = new StringContent(
                JsonSerializer.Serialize(createDto),
                Encoding.UTF8,
                "application/json");
            var createResponse = await client.PostAsync("/api/v1.0/products", createContent);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdContent = await createResponse.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(createdContent);
            var productId = doc.RootElement.GetProperty("id").GetInt32();

            // Act
            var deleteResponse = await client.DeleteAsync($"/api/v1.0/products/{productId}");

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.DeleteAsync("/api/v1.0/products/99999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
