using DemoProductsWebAPI.Common.DTOs;
using MediatR;

namespace DemoProductsWebAPI.Application.Products.Queries
{
    public record GetProductByIdQuery(int Id) : IRequest<ProductDto?>;
}
