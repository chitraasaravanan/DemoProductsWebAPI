using DemoProductsWebAPI.Application.DTOs;
using MediatR;

namespace DemoProductsWebAPI.Application.Products.Commands
{
    public record CreateProductCommand(ProductDto Product) : IRequest<ProductDto>;
}
