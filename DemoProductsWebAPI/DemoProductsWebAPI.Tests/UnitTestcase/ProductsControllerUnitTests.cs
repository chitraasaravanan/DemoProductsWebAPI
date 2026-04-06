using Xunit;
using Moq;
using DemoProductsWebAPI.API.Controllers;
using DemoProductsWebAPI.Common.Interfaces;
using DemoProductsWebAPI.Common.DTOs;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using System.Collections.Generic;

namespace DemoProductsWebAPI.Tests.UnitTestcase
{
    public class ProductsControllerUnitTests
    {
        [Fact]
        public async System.Threading.Tasks.Task Get_Calls_Service_GetAll()
        {
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Queries.GetAllProductsQuery>(), It.IsAny<System.Threading.CancellationToken>())).Returns(System.Threading.Tasks.Task.FromResult<System.Collections.Generic.IEnumerable<ProductDto>>(new List<ProductDto>()));
            var controller = new ProductsController(mediatorMock.Object);
            var res = await controller.Get();
            mediatorMock.Verify(m => m.Send(It.IsAny<Application.Products.Queries.GetAllProductsQuery>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            res.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async System.Threading.Tasks.Task GetById_Calls_Service_GetById()
        {
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Queries.GetProductByIdQuery>(), It.IsAny<System.Threading.CancellationToken>())).Returns(System.Threading.Tasks.Task.FromResult<ProductDto?>(new ProductDto { Id = 1 }));
            var controller = new ProductsController(mediatorMock.Object);
            var res = await controller.GetById(1);
            mediatorMock.Verify(m => m.Send(It.IsAny<Application.Products.Queries.GetProductByIdQuery>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            res.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async System.Threading.Tasks.Task Create_Calls_Service_Create()
        {
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Commands.CreateProductCommand>(), It.IsAny<System.Threading.CancellationToken>())).Returns(System.Threading.Tasks.Task.FromResult<ProductDto>(new ProductDto { Id = 5 }));
            var controller = new ProductsController(mediatorMock.Object);
            var dto = new ProductDto { ProductName = "x" };
            var res = await controller.Create(dto);
            mediatorMock.Verify(m => m.Send(It.IsAny<Application.Products.Commands.CreateProductCommand>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            res.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async System.Threading.Tasks.Task Update_Calls_Service_Update()
        {
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Commands.UpdateProductCommand>(), It.IsAny<System.Threading.CancellationToken>())).Returns(System.Threading.Tasks.Task.FromResult<bool>(true));
            var controller = new ProductsController(mediatorMock.Object);
            var dto = new ProductDto { Id = 1, ProductName = "x" };
            var res = await controller.Update(1, dto);
            mediatorMock.Verify(m => m.Send(It.IsAny<Application.Products.Commands.UpdateProductCommand>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            res.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async System.Threading.Tasks.Task Delete_Calls_Service_Delete()
        {
            var mediatorMock = new Mock<MediatR.IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<Application.Products.Commands.DeleteProductCommand>(), It.IsAny<System.Threading.CancellationToken>())).Returns(System.Threading.Tasks.Task.FromResult<bool>(true));
            var controller = new ProductsController(mediatorMock.Object);
            var res = await controller.Delete(1);
            mediatorMock.Verify(m => m.Send(It.IsAny<Application.Products.Commands.DeleteProductCommand>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            res.Should().BeOfType<NoContentResult>();
        }
    }
}
