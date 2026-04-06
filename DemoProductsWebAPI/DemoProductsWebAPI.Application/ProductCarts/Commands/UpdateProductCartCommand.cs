using MediatR;
using DemoProductsWebAPI.Common.DTOs;

namespace DemoProductsWebAPI.Application.ProductCarts.Commands
{
    public record UpdateProductCartCommand(ProductCartDto ProductCart) : IRequest<bool>;
}
