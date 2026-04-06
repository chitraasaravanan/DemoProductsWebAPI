using DemoProductsWebAPI.Common.DTOs;
using MediatR;

namespace DemoProductsWebAPI.Application.Products.Queries
{
    public record GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>;
}
