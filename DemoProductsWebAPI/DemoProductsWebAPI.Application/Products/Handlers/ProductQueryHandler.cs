using MediatR;
using DemoProductsWebAPI.Application.Products.Queries;
using DemoProductsWebAPI.Application.Interfaces;
using DemoProductsWebAPI.Application.DTOs;

namespace DemoProductsWebAPI.Application.Products.Handlers
{
    public class ProductQueryHandler : 
        IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>,
        IRequestHandler<GetProductByIdQuery, ProductDto?>
    {
        private readonly IProductReadService _readService;

        public ProductQueryHandler(IProductReadService readService)
        {
            _readService = readService;
        }

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
