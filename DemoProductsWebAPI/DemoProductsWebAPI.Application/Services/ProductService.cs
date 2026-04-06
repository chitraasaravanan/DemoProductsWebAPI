using AutoMapper;
using DemoProductsWebAPI.Common.DTOs;
using DemoProductsWebAPI.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace DemoProductsWebAPI.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;
        private readonly MediatR.IMediator _mediator;

        private static readonly Action<ILogger, string, Exception?> _creatingProduct =
            Microsoft.Extensions.Logging.LoggerMessage.Define<string>(Microsoft.Extensions.Logging.LogLevel.Information, new Microsoft.Extensions.Logging.EventId(1001, nameof(CreateAsync)), "Creating product {ProductName}");

        private static readonly Action<ILogger, int, Exception?> _createdProduct =
            Microsoft.Extensions.Logging.LoggerMessage.Define<int>(Microsoft.Extensions.Logging.LogLevel.Information, new Microsoft.Extensions.Logging.EventId(1002, nameof(CreateAsync)), "Created product {ProductId}");

        public ProductService(IMapper mapper, ILogger<ProductService> logger, MediatR.IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<ProductDto>> BulkInsertAsync(IEnumerable<ProductDto> dtos, CancellationToken cancellationToken = default)
        {
            if (dtos == null) return Enumerable.Empty<ProductDto>();
            if (dtos == null) return Enumerable.Empty<ProductDto>();
            var results = new List<ProductDto>();
            foreach (var d in dtos)
            {
                var created = await CreateAsync(d, cancellationToken).ConfigureAwait(false);
                results.Add(created);
            }
            return results;
        }

        public async Task<ProductDto> CreateAsync(ProductDto dto, CancellationToken cancellationToken = default)
        {
            _creatingProduct(_logger, dto.ProductName, null);
            var result = await _mediator.Send(new Application.Products.Commands.CreateProductCommand(dto), cancellationToken).ConfigureAwait(false);
            _createdProduct(_logger, result.Id, null);
            return result;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting product {ProductId}", id);
            var res = await _mediator.Send(new Application.Products.Commands.DeleteProductCommand(id), cancellationToken).ConfigureAwait(false);
            if (!res) _logger.LogWarning("Product {ProductId} not found for delete", id);
            else _logger.LogInformation("Deleted product {ProductId}", id);
            return res;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all products");
            var list = await _mediator.Send(new Application.Products.Queries.GetAllProductsQuery(), cancellationToken).ConfigureAwait(false);
            return list;
        }

        public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving product {ProductId}", id);
            var product = await _mediator.Send(new Application.Products.Queries.GetProductByIdQuery(id), cancellationToken).ConfigureAwait(false);
            return product;
        }

        public async Task<bool> UpdateAsync(ProductDto dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating product {ProductId}", dto.Id);
            var res = await _mediator.Send(new Application.Products.Commands.UpdateProductCommand(dto), cancellationToken).ConfigureAwait(false);
            if (!res) _logger.LogWarning("Product {ProductId} not found for update", dto.Id);
            else _logger.LogInformation("Updated product {ProductId}", dto.Id);
            return res;
        }
    }
}
