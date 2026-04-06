using DemoProductsWebAPI.Application.DTOs;
using MediatR;

namespace DemoProductsWebAPI.Application.Products.Commands
{
    public record UpdateProductCommand(ProductDto Product) : IRequest<bool>;
}
