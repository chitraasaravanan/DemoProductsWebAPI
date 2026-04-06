using MediatR;
using DemoProductsWebAPI.Application.DTOs;

namespace DemoProductsWebAPI.Application.ProductCarts.Commands
{
    public record CreateProductCartCommand(ProductCartDto ProductCart) : IRequest<ProductCartDto>;
}
