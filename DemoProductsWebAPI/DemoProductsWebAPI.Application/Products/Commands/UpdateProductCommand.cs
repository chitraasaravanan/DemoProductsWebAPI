using DemoProductsWebAPI.Common.DTOs;
using MediatR;

namespace DemoProductsWebAPI.Application.Products.Commands
{
    public record UpdateProductCommand(ProductDto Product) : IRequest<bool>;
}
