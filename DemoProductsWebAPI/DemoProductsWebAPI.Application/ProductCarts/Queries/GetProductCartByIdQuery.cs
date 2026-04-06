using MediatR;
using DemoProductsWebAPI.Application.DTOs;

namespace DemoProductsWebAPI.Application.ProductCarts.Queries
{
    public record GetProductCartByIdQuery(int Id) : IRequest<ProductCartDto?>;
}
