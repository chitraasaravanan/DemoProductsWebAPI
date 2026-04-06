using MediatR;
using DemoProductsWebAPI.Common.DTOs;

namespace DemoProductsWebAPI.Application.ProductCarts.Queries
{
    public record GetProductCartByIdQuery(int Id) : IRequest<ProductCartDto?>;
}
