using DemoProductsWebAPI.Application.Products.Commands;
using DemoProductsWebAPI.Common.DTOs;
using DemoProductsWebAPI.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DemoProductsWebAPI.Application.Products.Handlers
{
    public class ProductCommandHandler(IUnitOfWork uow, AutoMapper.IMapper mapper, ILogger<ProductCommandHandler> logger, MediatR.IMediator mediator) :
        IRequestHandler<CreateProductCommand, ProductDto>,
        IRequestHandler<UpdateProductCommand, bool>,
        IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IUnitOfWork _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        private readonly AutoMapper.IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly ILogger<ProductCommandHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly MediatR.IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating product {ProductName}", request.Product.ProductName);
            var entity = _mapper.Map<Domain.Entities.Product>(request.Product);
            entity.CreatedOn = request.Product.CreatedOn == default ? DateTime.UtcNow : request.Product.CreatedOn;
            await _uow.Products.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            var result = _mapper.Map<ProductDto>(entity);
            _logger.LogInformation("Created product {ProductId}", result.Id);
            await _mediator.Publish(new Products.Notifications.ProductCreatedNotification(result), cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating product {ProductId}", request.Product.Id);
            var existing = await _uow.Products.GetByIdAsync(request.Product.Id, cancellationToken).ConfigureAwait(false);
            if (existing == null) return false;
            _mapper.Map(request.Product, existing);
            existing.ModifiedOn = request.Product.ModifiedOn ?? DateTime.UtcNow;
            _uow.Products.Update(existing);
            await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Updated product {ProductId}", request.Product.Id);
            return true;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting product {ProductId}", request.Id);
            var existing = await _uow.Products.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);
            if (existing == null) return false;
            _uow.Products.Remove(existing);
            await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Deleted product {ProductId}", request.Id);
            return true;
        }
    }
}
