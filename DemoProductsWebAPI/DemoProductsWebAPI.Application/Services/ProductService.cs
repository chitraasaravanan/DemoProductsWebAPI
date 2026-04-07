using AutoMapper;
using DemoProductsWebAPI.Common.DTOs;
using DemoProductsWebAPI.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace DemoProductsWebAPI.Application.Services
{
    /// <summary>
    /// Service that provides business logic for Product operations using CQRS pattern via MediatR.
    /// Delegates all data operations to CQRS handlers (Commands and Queries) through the mediator.
    /// Logs all operations for observability and debugging purposes.
    /// 
    /// Primary responsibilities:
    /// - Orchestrate Create, Read, Update, Delete operations
    /// - Log operation start, completion, and failure states
    /// - Support bulk insert operations through sequential single inserts
    /// </summary>
    public class ProductService(IMapper mapper, ILogger<ProductService> logger, MediatR.IMediator mediator) : IProductService
    {
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly ILogger<ProductService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly MediatR.IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        private static readonly Action<ILogger, string, Exception?> _creatingProduct =
            Microsoft.Extensions.Logging.LoggerMessage.Define<string>(Microsoft.Extensions.Logging.LogLevel.Information, new Microsoft.Extensions.Logging.EventId(1001, nameof(CreateAsync)), "Creating product {ProductName}");

        private static readonly Action<ILogger, int, Exception?> _createdProduct =
            Microsoft.Extensions.Logging.LoggerMessage.Define<int>(Microsoft.Extensions.Logging.LogLevel.Information, new Microsoft.Extensions.Logging.EventId(1002, nameof(CreateAsync)), "Created product {ProductId}");

        /// <summary>
        /// Inserts multiple products into the database by invoking CreateAsync for each item sequentially.
        /// Flow: Validate input → Iterate items → Call CreateAsync for each → Collect results → Return list.
        /// </summary>
        /// <param name="dtos">Collection of ProductDto objects to insert</param>
        /// <param name="cancellationToken">Token to cancel the operation</param>
        /// <returns>Collection of created ProductDto objects with assigned IDs, or empty if input null</returns>
        public async Task<IEnumerable<ProductDto>> BulkInsertAsync(IEnumerable<ProductDto> dtos, CancellationToken cancellationToken = default)
        {
            // Return empty collection if input is null
            if (dtos == null) return [];
            if (dtos == null) return [];

            var results = new List<ProductDto>();
            foreach (var d in dtos)
            {
                // Create each product and collect results
                var created = await CreateAsync(d, cancellationToken).ConfigureAwait(false);
                results.Add(created);
            }
            return results;
        }

        /// <summary>
        /// Creates a new product by sending a CreateProductCommand through the mediator.
        /// Flow: Log start → Send command to handler → Log completion with assigned ID → Return DTO.
        /// </summary>
        /// <param name="dto">ProductDto containing product details to create</param>
        /// <param name="cancellationToken">Token to cancel the operation</param>
        /// <returns>Created ProductDto with assigned Id from database</returns>
        public async Task<ProductDto> CreateAsync(ProductDto dto, CancellationToken cancellationToken = default)
        {
            // Log operation start with product name
            _creatingProduct(_logger, dto.ProductName, null);

            // Send CreateProductCommand to handler via mediator
            var result = await _mediator.Send(new Application.Products.Commands.CreateProductCommand(dto), cancellationToken).ConfigureAwait(false);

            // Log successful creation with assigned product ID
            _createdProduct(_logger, result.Id, null);
            return result;
        }

        /// <summary>
        /// Deletes a product by ID using DeleteProductCommand through the mediator.
        /// Flow: Log delete attempt → Send command → Log success or warning if not found → Return result.
        /// </summary>
        /// <param name="id">The product ID to delete</param>
        /// <param name="cancellationToken">Token to cancel the operation</param>
        /// <returns>True if deletion succeeded; false if product not found</returns>
        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            // Log deletion attempt
            _logger.LogInformation("Deleting product {ProductId}", id);

            // Send DeleteProductCommand to handler via mediator
            var res = await _mediator.Send(new Application.Products.Commands.DeleteProductCommand(id), cancellationToken).ConfigureAwait(false);

            // Log result - warning if not found, success if deleted
            if (!res) _logger.LogWarning("Product {ProductId} not found for delete", id);
            else _logger.LogInformation("Deleted product {ProductId}", id);
            return res;
        }

        /// <summary>
        /// Retrieves all products from the database using GetAllProductsQuery through the mediator.
        /// Flow: Send query to handler → Log retrieval → Return collection of DTOs.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation</param>
        /// <returns>Collection of all ProductDtos; empty if no products exist</returns>
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
