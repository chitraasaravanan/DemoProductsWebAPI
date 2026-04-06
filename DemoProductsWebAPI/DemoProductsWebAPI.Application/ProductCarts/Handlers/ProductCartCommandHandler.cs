using MediatR;
using DemoProductsWebAPI.Application.Interfaces;
using DemoProductsWebAPI.Application.ProductCarts.Commands;
using DemoProductsWebAPI.Application.DTOs;

namespace DemoProductsWebAPI.Application.ProductCarts.Handlers
{
    public class ProductCartCommandHandler :
        IRequestHandler<CreateProductCartCommand, ProductCartDto>,
        IRequestHandler<UpdateProductCartCommand, bool>,
        IRequestHandler<DeleteProductCartCommand, bool>
    {
        private readonly IProductCartService _service;

        public ProductCartCommandHandler(IProductCartService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<ProductCartDto> Handle(CreateProductCartCommand request, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(request.ProductCart, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> Handle(UpdateProductCartCommand request, CancellationToken cancellationToken)
        {
            return await _service.UpdateAsync(request.ProductCart, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> Handle(DeleteProductCartCommand request, CancellationToken cancellationToken)
        {
            return await _service.DeleteAsync(request.Id, cancellationToken).ConfigureAwait(false);
        }
    }
}
