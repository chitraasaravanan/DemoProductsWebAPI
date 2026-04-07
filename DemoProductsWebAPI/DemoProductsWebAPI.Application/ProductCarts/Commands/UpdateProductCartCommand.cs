using DemoProductsWebAPI.Common.DTOs;
using MediatR;

namespace DemoProductsWebAPI.Application.ProductCarts.Commands
{
    public record UpdateProductCartCommand(ProductCartDto ProductCart) : IRequest<bool>;
}
