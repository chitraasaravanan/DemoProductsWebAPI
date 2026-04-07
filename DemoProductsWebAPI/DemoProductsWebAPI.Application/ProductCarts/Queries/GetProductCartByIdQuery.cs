using DemoProductsWebAPI.Common.DTOs;
using MediatR;

namespace DemoProductsWebAPI.Application.ProductCarts.Queries
{
    public record GetProductCartByIdQuery(int Id) : IRequest<ProductCartDto?>;
}
