using MediatR;
using DemoProductsWebAPI.Application.DTOs;

namespace DemoProductsWebAPI.Application.ProductCarts.Commands
{
    public record UpdateProductCartCommand(ProductCartDto ProductCart) : IRequest<bool>;
}
