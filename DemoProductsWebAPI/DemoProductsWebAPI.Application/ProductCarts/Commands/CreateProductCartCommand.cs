using MediatR;
using DemoProductsWebAPI.Common.DTOs;

namespace DemoProductsWebAPI.Application.ProductCarts.Commands
{
    public record CreateProductCartCommand(ProductCartDto ProductCart) : IRequest<ProductCartDto>;
}
