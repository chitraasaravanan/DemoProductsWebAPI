using AutoMapper;
using DemoProductsWebAPI.Application.Products.Queries;
using DemoProductsWebAPI.Application.Services;
using DemoProductsWebAPI.Common.DTOs;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace DemoProductsWebAPI.Tests.UnitTestcase
{
    /// <summary>
    /// Unit tests for <see cref="ProductService"/> to verify business logic and mediator integration.
    /// Tests cover all service methods: Create, Read, Update, Delete, and BulkInsert operations.
    /// Uses mock mediator to isolate service logic from CQRS handlers and databases.
    /// </summary>
    public class ProductServiceTests
    {
        /// <summary>
        /// Verifies that CreateAsync sends a CreateProductCommand through the mediator.
        /// Flow: Create DTO → Call CreateAsync → Mediator sends command → Assert result has assigned ID.
        /// </summary>
        [Fact]
        public async Task CreateAsync_CallsMediatorWithCreateCommand()
        {
            // Arrange: Setup mock mediator to return newly created product with ID
            var createdDto = new ProductDto { Id = 1, ProductName = "Test", CreatedBy = "test", CreatedOn = DateTime.UtcNow };
            var mapperMock = new Mock<IMapper>();
            var logger = new NullLogger<ProductService>();
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Commands.CreateProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdDto);

            var service = new ProductService(mapperMock.Object, logger, mediatorMock.Object);
            var dto = new ProductDto { ProductName = "Test", CreatedBy = "test", CreatedOn = DateTime.UtcNow };

            // Act: Call CreateAsync with product data
            var result = await service.CreateAsync(dto, CancellationToken.None);

            // Assert: Verify result contains ID and mediator was called
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.ProductName.Should().Be("Test");
            mediatorMock.Verify(m => m.Send(It.IsAny<Application.Products.Commands.CreateProductCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Verifies that GetAllAsync sends a GetAllProductsQuery through the mediator.
        /// Flow: Call GetAllAsync → Mediator executes query → Assert list of products returned.
        /// </summary>
        [Fact]
        public async Task GetAllAsync_CallsMediatorWithGetAllQuery()
        {
            // Arrange: Setup mock mediator to return list of products
            var products = new List<ProductDto>
            {
                new() { Id = 1, ProductName = "Product1", CreatedBy = "test", CreatedOn = DateTime.UtcNow }
            };
            var mapperMock = new Mock<IMapper>();
            var logger = new NullLogger<ProductService>();
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<ProductDto>)products);

            var service = new ProductService(mapperMock.Object, logger, mediatorMock.Object);

            // Act
            var result = await service.GetAllAsync(CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            mediatorMock.Verify(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_CallsMediatorWithGetByIdQuery()
        {
            // Arrange
            var productDto = new ProductDto { Id = 1, ProductName = "Product1", CreatedBy = "test", CreatedOn = DateTime.UtcNow };
            var mapperMock = new Mock<IMapper>();
            var logger = new NullLogger<ProductService>();
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Queries.GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(productDto);

            var service = new ProductService(mapperMock.Object, logger, mediatorMock.Object);

            // Act
            var result = await service.GetByIdAsync(1, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            var mapperMock = new Mock<IMapper>();
            var logger = new NullLogger<ProductService>();
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Queries.GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductDto?)null);

            var service = new ProductService(mapperMock.Object, logger, mediatorMock.Object);

            // Act
            var result = await service.GetByIdAsync(999, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_CallsMediatorWithUpdateCommand()
        {
            // Arrange
            var mapperMock = new Mock<IMapper>();
            var logger = new NullLogger<ProductService>();
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Commands.UpdateProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var service = new ProductService(mapperMock.Object, logger, mediatorMock.Object);
            var dto = new ProductDto { Id = 1, ProductName = "Updated", CreatedBy = "test", CreatedOn = DateTime.UtcNow };

            // Act
            var result = await service.UpdateAsync(dto, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            mediatorMock.Verify(m => m.Send(It.IsAny<Application.Products.Commands.UpdateProductCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsFalse_WhenUpdateFails()
        {
            // Arrange
            var mapperMock = new Mock<IMapper>();
            var logger = new NullLogger<ProductService>();
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Commands.UpdateProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var service = new ProductService(mapperMock.Object, logger, mediatorMock.Object);
            var dto = new ProductDto { Id = 999, ProductName = "Updated", CreatedBy = "test", CreatedOn = DateTime.UtcNow };

            // Act
            var result = await service.UpdateAsync(dto, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAsync_CallsMediatorWithDeleteCommand()
        {
            // Arrange
            var mapperMock = new Mock<IMapper>();
            var logger = new NullLogger<ProductService>();
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Commands.DeleteProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var service = new ProductService(mapperMock.Object, logger, mediatorMock.Object);

            // Act
            var result = await service.DeleteAsync(1, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            mediatorMock.Verify(m => m.Send(It.IsAny<Application.Products.Commands.DeleteProductCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenDeleteFails()
        {
            // Arrange
            var mapperMock = new Mock<IMapper>();
            var logger = new NullLogger<ProductService>();
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Commands.DeleteProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var service = new ProductService(mapperMock.Object, logger, mediatorMock.Object);

            // Act
            var result = await service.DeleteAsync(999, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task BulkInsertAsync_ReturnsEmptyList_WhenInputIsNull()
        {
            // Arrange
            var mapperMock = new Mock<IMapper>();
            var logger = new NullLogger<ProductService>();
            var mediatorMock = new Mock<MediatR.IMediator>();

            var service = new ProductService(mapperMock.Object, logger, mediatorMock.Object);

            // Act
            var result = await service.BulkInsertAsync(null!, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task BulkInsertAsync_InsertsMultipleProducts()
        {
            // Arrange
            var dtos = new List<ProductDto>
            {
                new() { ProductName = "Product1", CreatedBy = "test", CreatedOn = DateTime.UtcNow },
                new() { ProductName = "Product2", CreatedBy = "test", CreatedOn = DateTime.UtcNow }
            };
            var createdDtos = new List<ProductDto>
            {
                new() { Id = 1, ProductName = "Product1", CreatedBy = "test", CreatedOn = DateTime.UtcNow },
                new() { Id = 2, ProductName = "Product2", CreatedBy = "test", CreatedOn = DateTime.UtcNow }
            };

            var mapperMock = new Mock<IMapper>();
            var logger = new NullLogger<ProductService>();
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Commands.CreateProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Application.Products.Commands.CreateProductCommand cmd, CancellationToken _) =>
                {
                    var matching = createdDtos.FirstOrDefault(d => d.ProductName == cmd.Product.ProductName);
                    return matching ?? createdDtos[0];
                });

            var service = new ProductService(mapperMock.Object, logger, mediatorMock.Object);

            // Act
            var result = await service.BulkInsertAsync(dtos, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(2);
        }
    }
}
