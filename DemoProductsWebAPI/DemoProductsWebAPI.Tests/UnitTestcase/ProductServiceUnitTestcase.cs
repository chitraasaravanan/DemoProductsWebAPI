using System;
using Xunit;
using FluentAssertions;
using Moq;
using DemoProductsWebAPI.Application.Services;
using DemoProductsWebAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using DemoProductsWebAPI.Application.DTOs;
using DemoProductsWebAPI.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using AutoMapper;
using System.Threading;

namespace DemoProductsWebAPI.Tests.UnitTestcase
{
    public class ProductServiceUnitTestcase
    {
        private static ApplicationDbContext CreateInMemoryDb(string name)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: name)
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateAsync_AddsProduct_ReturnsCreatedDto()
        {
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
                .Returns((Product p) => new ProductDto { Id = p.Id, ProductName = p.ProductName, CreatedBy = p.CreatedBy, CreatedOn = p.CreatedOn });

            var logger = new NullLogger<ProductService>();
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Commands.CreateProductCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProductDto { Id = 1, ProductName = "Test" });
            var svc = new ProductService(mapperMock.Object, logger, mediatorMock.Object);

            var dto = new ProductDto { ProductName = "Test", CreatedBy = "unit", CreatedOn = DateTime.UtcNow };
            var created = await svc.CreateAsync(dto, CancellationToken.None);

            created.Should().NotBeNull();
            created.ProductName.Should().Be("Test");
        }

        [Fact]
        public async System.Threading.Tasks.Task GetAllAsync_Returns_MappedList()
        {
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
                .Returns((Product p) => new ProductDto { Id = p.Id, ProductName = p.ProductName, CreatedBy = p.CreatedBy, CreatedOn = p.CreatedOn });

            var logger = new NullLogger<ProductService>();
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Queries.GetAllProductsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<ProductDto>
            {
                new ProductDto { Id = 1, ProductName = "p1", CreatedBy = "t", CreatedOn = DateTime.UtcNow }
            } as IEnumerable<ProductDto>);

            var svc = new ProductService(mapperMock.Object, logger, mediatorMock.Object);

            var list = await svc.GetAllAsync(CancellationToken.None);
            list.Should().NotBeNull();
            list.Should().HaveCountGreaterOrEqualTo(1);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetByIdAsync_Returns_Null_When_Not_Found()
        {
            var mapperMock = new Mock<IMapper>();
            var logger = new NullLogger<ProductService>();
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Queries.GetProductByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((ProductDto?)null);
            var svc = new ProductService(mapperMock.Object, logger, mediatorMock.Object);

            var res = await svc.GetByIdAsync(999, CancellationToken.None);
            res.Should().BeNull();
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateAsync_Returns_False_When_NotFound()
        {
            var mapperMock = new Mock<IMapper>();
            var logger = new NullLogger<ProductService>();
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Commands.UpdateProductCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
            var svc = new ProductService(mapperMock.Object, logger, mediatorMock.Object);

            var dto = new ProductDto { Id = 12345, ProductName = "x" };
            var res = await svc.UpdateAsync(dto, CancellationToken.None);
            res.Should().BeFalse();
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteAsync_Returns_False_When_NotFound()
        {
            var mapperMock = new Mock<IMapper>();
            var logger = new NullLogger<ProductService>();
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Commands.DeleteProductCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
            var svc = new ProductService(mapperMock.Object, logger, mediatorMock.Object);

            var res = await svc.DeleteAsync(999, CancellationToken.None);
            res.Should().BeFalse();
        }
    }
}
