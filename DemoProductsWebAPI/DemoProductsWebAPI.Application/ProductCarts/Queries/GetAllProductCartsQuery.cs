using MediatR;
using DemoProductsWebAPI.Application.DTOs;
using System.Collections.Generic;

namespace DemoProductsWebAPI.Application.ProductCarts.Queries
{
    public record GetAllProductCartsQuery() : IRequest<IEnumerable<ProductCartDto>>;
}
