using System;
using Microsoft.EntityFrameworkCore;
using Products = DemoProductsWebAPI.Application.Products;
using Xunit;
using Moq;
using FluentAssertions;
using DemoProductsWebAPI.Application.Products.Handlers;
using DemoProductsWebAPI.Application.DTOs;
using System.Threading;
using System.Collections.Generic;
using DemoProductsWebAPI.Common.Interfaces;

namespace DemoProductsWebAPI.Tests.UnitTestcase
{
    public class ProductHandlersUnitTestcase
    {
        [Fact]
        public async System.Threading.Tasks.Task ProductQueryHandler_GetAll_ReturnsList()
        {
            var mockRead = new Mock<IProductReadService>();
            mockRead.Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<ProductDto>
            {
                new ProductDto { Id = 1, ProductName = "P1", CreatedBy = "t", CreatedOn = DateTime.UtcNow }
            });

            var handler = new ProductQueryHandler(mockRead.Object);
            var result = await handler.Handle(new Application.Products.Queries.GetAllProductsQuery(), CancellationToken.None);
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Fact]
        public async System.Threading.Tasks.Task ProductCommandHandler_Create_PersistsEntity_And_PublishesNotification()
        {
            // arrange - use in-memory db and real UnitOfWork so handler exercises persistence
            var options = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<DemoProductsWebAPI.Infrastructure.Data.ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "ph_create")
                .Options;
            await using var db = new DemoProductsWebAPI.Infrastructure.Data.ApplicationDbContext(options);
            var uow = new DemoProductsWebAPI.Infrastructure.Data.UnitOfWork(db);

            var mapperMock = new Mock<AutoMapper.IMapper>();
            mapperMock.Setup(m => m.Map<Domain.Entities.Product>(It.IsAny<ProductDto>()))
                .Returns((ProductDto dto) => new Domain.Entities.Product { ProductName = dto.ProductName, CreatedBy = dto.CreatedBy, CreatedOn = dto.CreatedOn });
            mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Domain.Entities.Product>()))
                .Returns((Domain.Entities.Product p) => new ProductDto { Id = p.Id, ProductName = p.ProductName, CreatedBy = p.CreatedBy, CreatedOn = p.CreatedOn });

            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ProductCommandHandler>();
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Publish(It.IsAny<MediatR.INotification>(), It.IsAny<CancellationToken>())).Returns(System.Threading.Tasks.Task.CompletedTask);

            var handler = new ProductCommandHandler(uow, mapperMock.Object, logger, mediatorMock.Object);
            var dto = new ProductDto { ProductName = "New", CreatedBy = "t", CreatedOn = DateTime.UtcNow };

            // act
            var result = await handler.Handle(new Application.Products.Commands.CreateProductCommand(dto), CancellationToken.None);

            // assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            var persisted = await db.Products.FindAsync(result.Id);
            persisted.Should().NotBeNull();
            mediatorMock.Verify(m => m.Publish(It.IsAny<Application.Products.Notifications.ProductCreatedNotification>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
