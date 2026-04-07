using DemoProductsWebAPI.API.Controllers;
using DemoProductsWebAPI.Common.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DemoProductsWebAPI.Tests.UnitTestcase
{
    /// <summary>
    /// Unit tests for <see cref="ProductsController"/> to verify HTTP endpoint behavior.
    /// Tests cover all CRUD operations (Create, Read, Update, Delete) with success and failure scenarios.
    /// Uses Moq for mocking the MediatR mediator and FluentAssertions for readable assertions.
    /// </summary>
    public class ProductsControllerUnitTests
    {
        /// <summary>
        /// Verifies that the Get endpoint returns 200 OK with a list of products from the mediator.
        /// Flow: Creates a mock mediator that returns empty list → calls Get() → asserts OkObjectResult is returned.
        /// </summary>
        [Fact]
        public async Task Get_ReturnsOkResult_WithProductList()
        {
            // Arrange: Setup mock mediator to return empty product list
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Queries.GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);
            var controller = new ProductsController(mediatorMock.Object);

            // Act: Invoke the Get endpoint
            var result = await controller.Get();

            // Assert: Verify response type and mediator was called exactly once
            result.Should().BeOfType<OkObjectResult>();
            mediatorMock.Verify(m => m.Send(It.IsAny<Application.Products.Queries.GetAllProductsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Verifies that GetById endpoint returns 200 OK when a product is found.
        /// Flow: Sets up mock mediator to return a product DTO → calls GetById(1) → asserts successful response.
        /// </summary>
        [Fact]
        public async Task GetById_ReturnsOkResult_WhenProductExists()
        {
            // Arrange: Setup mock mediator to return a specific product
            var dto = new ProductDto { Id = 1, ProductName = "Test", CreatedBy = "test", CreatedOn = DateTime.UtcNow };
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Queries.GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dto);
            var controller = new ProductsController(mediatorMock.Object);

            // Act: Invoke GetById with valid product ID
            var result = await controller.GetById(1);

            // Assert: Verify 200 OK response and mediator invoked
            result.Should().BeOfType<OkObjectResult>();
            mediatorMock.Verify(m => m.Send(It.IsAny<Application.Products.Queries.GetProductByIdQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Verifies that GetById endpoint returns 404 NotFound when product does not exist.
        /// Flow: Mock mediator returns null → calls GetById(999) → asserts 404 response.
        /// </summary>
        [Fact]
        public async Task GetById_ReturnsNotFound_WhenProductNotFound()
        {
            // Arrange: Setup mock mediator to return null (product not found)
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Queries.GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductDto?)null);
            var controller = new ProductsController(mediatorMock.Object);

            // Act: Invoke GetById with non-existent product ID
            var result = await controller.GetById(999);

            // Assert: Verify 404 NotFound response
            result.Should().BeOfType<NotFoundResult>();
        }

        /// <summary>
        /// Verifies that Create endpoint returns 201 Created with the created product and location header.
        /// Flow: Mock mediator creates and returns product → calls Create(dto) → asserts CreatedAtActionResult.
        /// </summary>
        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WithValidDto()
        {
            // Arrange: Setup mock mediator to return newly created product with assigned ID
            var dto = new ProductDto { ProductName = "New Product", CreatedBy = "test", CreatedOn = DateTime.UtcNow };
            var createdDto = new ProductDto { Id = 1, ProductName = "New Product", CreatedBy = "test", CreatedOn = DateTime.UtcNow };
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Commands.CreateProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdDto);
            var controller = new ProductsController(mediatorMock.Object);

            // Act: Invoke Create with valid product data
            var result = await controller.Create(dto);

            // Assert: Verify 201 Created with GetById location header and mediator called
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = (CreatedAtActionResult)result;
            createdResult.ActionName.Should().Be(nameof(ProductsController.GetById));
            mediatorMock.Verify(m => m.Send(It.IsAny<Application.Products.Commands.CreateProductCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Verifies that Update endpoint returns 204 NoContent when update succeeds.
        /// Flow: Mock mediator returns true (successful update) → calls Update(1, dto) → asserts 204 response.
        /// </summary>
        [Fact]
        public async Task Update_ReturnsNoContent_WhenUpdateSucceeds()
        {
            // Arrange: Setup mock mediator to return true (successful update)
            var dto = new ProductDto { Id = 1, ProductName = "Updated", CreatedBy = "test", CreatedOn = DateTime.UtcNow };
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Commands.UpdateProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var controller = new ProductsController(mediatorMock.Object);

            // Act: Invoke Update with matching product ID
            var result = await controller.Update(1, dto);

            // Assert: Verify 204 NoContent and mediator called
            result.Should().BeOfType<NoContentResult>();
            mediatorMock.Verify(m => m.Send(It.IsAny<Application.Products.Commands.UpdateProductCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Verifies that Update endpoint returns 400 BadRequest when URL ID does not match DTO ID.
        /// Flow: Mismatched IDs → calls Update(999, dto with Id=1) → asserts 400 before mediator call.
        /// </summary>
        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange: Create DTO with different ID than URL parameter
            var dto = new ProductDto { Id = 1, ProductName = "Updated" };
            var mediatorMock = new Mock<MediatR.IMediator>();
            var controller = new ProductsController(mediatorMock.Object);

            // Act: Invoke Update with mismatched ID (URL: 999, DTO: 1)
            var result = await controller.Update(999, dto);

            // Assert: Verify 400 BadRequest (validation before mediator call)
            result.Should().BeOfType<BadRequestResult>();
        }

        /// <summary>
        /// Verifies that Update endpoint returns 404 NotFound when product does not exist.
        /// Flow: Mock mediator returns false → calls Update(999, dto) → asserts 404 response.
        /// </summary>
        [Fact]
        public async Task Update_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange: Setup mock mediator to return false (product not found in database)
            var dto = new ProductDto { Id = 999, ProductName = "Updated" };
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Commands.UpdateProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            var controller = new ProductsController(mediatorMock.Object);

            // Act: Invoke Update with non-existent product ID
            var result = await controller.Update(999, dto);

            // Assert: Verify 404 NotFound response
            result.Should().BeOfType<NotFoundResult>();
        }

        /// <summary>
        /// Verifies that Delete endpoint returns 204 NoContent when deletion succeeds.
        /// Flow: Mock mediator returns true (successful delete) → calls Delete(1) → asserts 204 response.
        /// </summary>
        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleteSucceeds()
        {
            // Arrange: Setup mock mediator to return true (successful deletion)
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Commands.DeleteProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var controller = new ProductsController(mediatorMock.Object);

            // Act: Invoke Delete with valid product ID
            var result = await controller.Delete(1);

            // Assert: Verify 204 NoContent and mediator called
            result.Should().BeOfType<NoContentResult>();
            mediatorMock.Verify(m => m.Send(It.IsAny<Application.Products.Commands.DeleteProductCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Verifies that Delete endpoint returns 404 NotFound when product does not exist.
        /// Flow: Mock mediator returns false → calls Delete(99999) → asserts 404 response.
        /// </summary>
        [Fact]
        public async Task Delete_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange: Setup mock mediator to return false (product not found)
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Commands.DeleteProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            var controller = new ProductsController(mediatorMock.Object);

            // Act: Invoke Delete with non-existent product ID
            var result = await controller.Delete(999);

            // Assert: Verify 404 NotFound response
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
