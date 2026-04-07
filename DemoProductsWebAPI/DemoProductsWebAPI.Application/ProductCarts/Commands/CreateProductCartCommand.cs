using DemoProductsWebAPI.Common.DTOs;
using MediatR;

namespace DemoProductsWebAPI.Application.ProductCarts.Commands
{
    public record CreateProductCartCommand(ProductCartDto ProductCart) : IRequest<ProductCartDto>;
}
