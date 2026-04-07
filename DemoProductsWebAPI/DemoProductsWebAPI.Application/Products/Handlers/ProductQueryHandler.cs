using DemoProductsWebAPI.Application.Products.Queries;
using DemoProductsWebAPI.Common.DTOs;
using DemoProductsWebAPI.Common.Interfaces;
using MediatR;

namespace DemoProductsWebAPI.Application.Products.Handlers
{
    public class ProductQueryHandler(IProductReadRepository readService) :
        IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>,
        IRequestHandler<GetProductByIdQuery, ProductDto?>
    {
        private readonly IProductReadRepository _readService = readService;

        public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            return await _readService.GetAllAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            return await _readService.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);
        }
    }
}
