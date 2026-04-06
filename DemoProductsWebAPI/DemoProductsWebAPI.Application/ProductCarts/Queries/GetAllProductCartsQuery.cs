using MediatR;
using DemoProductsWebAPI.Common.DTOs;
using System.Collections.Generic;

namespace DemoProductsWebAPI.Application.ProductCarts.Queries
{
    public record GetAllProductCartsQuery() : IRequest<IEnumerable<ProductCartDto>>;
}
