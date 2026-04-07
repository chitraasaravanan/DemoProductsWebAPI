using DemoProductsWebAPI.Application.ProductCarts.Queries;
using DemoProductsWebAPI.Common.DTOs;
using DemoProductsWebAPI.Common.Interfaces;
using MediatR;

namespace DemoProductsWebAPI.Application.ProductCarts.Handlers
{
    public class ProductCartQueryHandler(IProductCartService service) :
        IRequestHandler<GetAllProductCartsQuery, IEnumerable<ProductCartDto>>,
        IRequestHandler<GetProductCartByIdQuery, ProductCartDto?>
    {
        private readonly IProductCartService _service = service ?? throw new ArgumentNullException(nameof(service));

        public async Task<IEnumerable<ProductCartDto>> Handle(GetAllProductCartsQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<ProductCartDto?> Handle(GetProductCartByIdQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);
        }
    }
}
