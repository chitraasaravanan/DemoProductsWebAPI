using DemoProductsWebAPI.Application.Products.Handlers;
using DemoProductsWebAPI.Common.DTOs;
using DemoProductsWebAPI.Common.Interfaces;
using DemoProductsWebAPI.Domain.Entities;
using DemoProductsWebAPI.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace DemoProductsWebAPI.Tests.UnitTestcase
{
    /// <summary>
    /// Unit tests for <see cref="ProductQueryHandler"/> that verify query handling for retrieving product data.
    /// Tests cover both successful retrieval scenarios and empty result cases.
    /// Uses mock IProductReadRepository to isolate handler logic from database operations.
    /// </summary>
    public class ProductQueryHandlerTests
    {
        /// <summary>
        /// Verifies that GetAllProductsQuery returns all products from the read repository.
        /// Flow: Mock repository returns list of 2 products → Handle query → Assert list returned with 2 items.
        /// </summary>
        [Fact]
        public async Task Handle_GetAllProductsQuery_ReturnsAllProducts()
        {
            // Arrange: Setup mock repository to return multiple product DTOs
            var products = new List<ProductDto>
            {
                new() { Id = 1, ProductName = "Product1", CreatedBy = "test", CreatedOn = DateTime.UtcNow },
                new() { Id = 2, ProductName = "Product2", CreatedBy = "test", CreatedOn = DateTime.UtcNow }
            };
            var mockRead = new Mock<IProductReadRepository>();
            mockRead.Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(products);
            var handler = new ProductQueryHandler(mockRead.Object);

            // Act: Execute GetAllProductsQuery through handler
            var result = await handler.Handle(new Application.Products.Queries.GetAllProductsQuery(), CancellationToken.None);

            // Assert: Verify all products returned and repository called once
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            mockRead.Verify(m => m.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Verifies that GetAllProductsQuery returns empty list when no products exist.
        /// Flow: Mock repository returns empty list → Handle query → Assert empty collection returned.
        /// </summary>
        [Fact]
        public async Task Handle_GetAllProductsQuery_ReturnsEmptyList_WhenNoProducts()
        {
            // Arrange: Setup mock repository to return empty list (no products in database)
            var mockRead = new Mock<IProductReadRepository>();
            mockRead.Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
            var handler = new ProductQueryHandler(mockRead.Object);

            // Act: Execute GetAllProductsQuery with no data
            var result = await handler.Handle(new Application.Products.Queries.GetAllProductsQuery(), CancellationToken.None);

            // Assert: Verify empty collection returned
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        /// <summary>
        /// Verifies that GetProductByIdQuery returns a specific product when found.
        /// Flow: Mock repository returns product with ID 1 → Handle query with ID 1 → Assert product data matches.
        /// </summary>
        [Fact]
        public async Task Handle_GetProductByIdQuery_ReturnsProduct_WhenFound()
        {
            // Arrange: Setup mock repository to return specific product by ID
            var product = new ProductDto { Id = 1, ProductName = "Product1", CreatedBy = "test", CreatedOn = DateTime.UtcNow };
            var mockRead = new Mock<IProductReadRepository>();
            mockRead.Setup(m => m.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(product);
            var handler = new ProductQueryHandler(mockRead.Object);

            // Act: Execute GetProductByIdQuery with ID 1
            var result = await handler.Handle(new Application.Products.Queries.GetProductByIdQuery(1), CancellationToken.None);

            // Assert: Verify product found with correct ID and name
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.ProductName.Should().Be("Product1");
            mockRead.Verify(m => m.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Verifies that GetProductByIdQuery returns null when product not found.
        /// Flow: Mock repository returns null → Handle query with non-existent ID → Assert null result.
        /// </summary>
        [Fact]
        public async Task Handle_GetProductByIdQuery_ReturnsNull_WhenNotFound()
        {
            // Arrange: Setup mock repository to return null (product doesn't exist)
            var mockRead = new Mock<IProductReadRepository>();
            mockRead.Setup(m => m.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((ProductDto?)null);
            var handler = new ProductQueryHandler(mockRead.Object);

            // Act: Execute GetProductByIdQuery with non-existent ID
            var result = await handler.Handle(new Application.Products.Queries.GetProductByIdQuery(999), CancellationToken.None);

            // Assert: Verify null returned
            result.Should().BeNull();
        }
    }

    /// <summary>
    /// Unit tests for <see cref="ProductCommandHandler"/> that verify command handling for creating, updating, and deleting products.
    /// Tests cover entity persistence, notifications, and failure scenarios.
    /// Uses in-memory database for data operations and mock mapper for DTO/Entity mapping.
    /// </summary>
    public class ProductCommandHandlerTests
    {
        /// <summary>
        /// Helper method to create an in-memory database context for isolated test scenarios.
        /// </summary>
        /// <param name="name">Unique database name to ensure test isolation</param>
        /// <returns>Configured ApplicationDbContext using InMemoryDatabase</returns>
        private static ApplicationDbContext CreateInMemoryDb(string name)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(name)
                .Options;
            return new ApplicationDbContext(options);
        }

        /// <summary>
        /// Verifies that CreateProductCommand persists the product to database and publishes a domain notification.
        /// Flow: Create DTO → Map to entity → Save to DB → Publish notification → Assert entity created with ID assigned.
        /// </summary>
        [Fact]
        public async Task Handle_CreateProductCommand_PersistsEntity_And_PublishesNotification()
        {
            // Arrange: Setup in-memory database and mock mapper for entity creation
            using var db = CreateInMemoryDb("test_create_product");
            var uow = new UnitOfWork(db);
            var mapperMock = new Mock<AutoMapper.IMapper>();
            mapperMock.Setup(m => m.Map<Domain.Entities.Product>(It.IsAny<ProductDto>()))
                .Returns((ProductDto dto) => new Product
                {
                    ProductName = dto.ProductName,
                    CreatedBy = dto.CreatedBy,
                    CreatedOn = dto.CreatedOn
                });
            mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
                .Returns((Product p) => new ProductDto
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    CreatedBy = p.CreatedBy,
                    CreatedOn = p.CreatedOn
                });

            var logger = new NullLogger<ProductCommandHandler>();
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Publish(It.IsAny<MediatR.INotification>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var handler = new ProductCommandHandler(uow, mapperMock.Object, logger, mediatorMock.Object);
            var dto = new ProductDto { ProductName = "New Product", CreatedBy = "test", CreatedOn = DateTime.UtcNow };

            // Act
            var result = await handler.Handle(new Application.Products.Commands.CreateProductCommand(dto), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.ProductName.Should().Be("New Product");
            var persisted = await db.Products.FindAsync(result.Id);
            persisted.Should().NotBeNull();
            persisted!.ProductName.Should().Be("New Product");
            mediatorMock.Verify(m => m.Publish(It.IsAny<Application.Products.Notifications.ProductCreatedNotification>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateProductCommand_UpdatesEntity_Successfully()
        {
            // Arrange
            using var db = CreateInMemoryDb("test_update_product");
            db.Products.Add(new Product { ProductName = "Original", CreatedBy = "test", CreatedOn = DateTime.UtcNow });
            await db.SaveChangesAsync();
            var productId = db.Products.First().Id;

            var uow = new UnitOfWork(db);
            var mapperMock = new Mock<AutoMapper.IMapper>();
            mapperMock.Setup(m => m.Map(It.IsAny<ProductDto>(), It.IsAny<Product>()))
                .Callback<ProductDto, Product>((dto, entity) =>
                {
                    entity.ProductName = dto.ProductName;
                    entity.ModifiedBy = dto.ModifiedBy;
                });

            var logger = new NullLogger<ProductCommandHandler>();
            var mediatorMock = new Mock<MediatR.IMediator>();

            var handler = new ProductCommandHandler(uow, mapperMock.Object, logger, mediatorMock.Object);
            var updateDto = new ProductDto { Id = productId, ProductName = "Updated", CreatedBy = "test", CreatedOn = DateTime.UtcNow };

            // Act
            var result = await handler.Handle(new Application.Products.Commands.UpdateProductCommand(updateDto), CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            var updated = await db.Products.FindAsync(productId);
            updated.Should().NotBeNull();
            updated!.ProductName.Should().Be("Updated");
        }

        [Fact]
        public async Task Handle_UpdateProductCommand_ReturnsFalse_WhenProductNotFound()
        {
            // Arrange
            using var db = CreateInMemoryDb("test_update_not_found");
            var uow = new UnitOfWork(db);
            var mapperMock = new Mock<AutoMapper.IMapper>();
            var logger = new NullLogger<ProductCommandHandler>();
            var mediatorMock = new Mock<MediatR.IMediator>();

            var handler = new ProductCommandHandler(uow, mapperMock.Object, logger, mediatorMock.Object);
            var updateDto = new ProductDto { Id = 999, ProductName = "Updated", CreatedBy = "test", CreatedOn = DateTime.UtcNow };

            // Act
            var result = await handler.Handle(new Application.Products.Commands.UpdateProductCommand(updateDto), CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_DeleteProductCommand_DeletesEntity_Successfully()
        {
            // Arrange
            using var db = CreateInMemoryDb("test_delete_product");
            db.Products.Add(new Product { ProductName = "ToDelete", CreatedBy = "test", CreatedOn = DateTime.UtcNow });
            await db.SaveChangesAsync();
            var productId = db.Products.First().Id;

            var uow = new UnitOfWork(db);
            var mapperMock = new Mock<AutoMapper.IMapper>();
            var logger = new NullLogger<ProductCommandHandler>();
            var mediatorMock = new Mock<MediatR.IMediator>();

            var handler = new ProductCommandHandler(uow, mapperMock.Object, logger, mediatorMock.Object);

            // Act
            var result = await handler.Handle(new Application.Products.Commands.DeleteProductCommand(productId), CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            var deleted = await db.Products.FindAsync(productId);
            deleted.Should().BeNull();
        }

        [Fact]
        public async Task Handle_DeleteProductCommand_ReturnsFalse_WhenProductNotFound()
        {
            // Arrange
            using var db = CreateInMemoryDb("test_delete_not_found");
            var uow = new UnitOfWork(db);
            var mapperMock = new Mock<AutoMapper.IMapper>();
            var logger = new NullLogger<ProductCommandHandler>();
            var mediatorMock = new Mock<MediatR.IMediator>();

            var handler = new ProductCommandHandler(uow, mapperMock.Object, logger, mediatorMock.Object);

            // Act
            var result = await handler.Handle(new Application.Products.Commands.DeleteProductCommand(999), CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }
    }
}
