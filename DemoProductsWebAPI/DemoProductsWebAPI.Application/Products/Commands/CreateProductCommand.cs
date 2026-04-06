using DemoProductsWebAPI.Common.DTOs;
using MediatR;

namespace DemoProductsWebAPI.Application.Products.Commands
{
    public record CreateProductCommand(ProductDto Product) : IRequest<ProductDto>;
}
