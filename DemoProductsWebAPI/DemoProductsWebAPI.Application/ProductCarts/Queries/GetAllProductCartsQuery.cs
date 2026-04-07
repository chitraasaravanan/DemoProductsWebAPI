using DemoProductsWebAPI.Common.DTOs;
using MediatR;

namespace DemoProductsWebAPI.Application.ProductCarts.Queries
{
    public record GetAllProductCartsQuery() : IRequest<IEnumerable<ProductCartDto>>;
}
