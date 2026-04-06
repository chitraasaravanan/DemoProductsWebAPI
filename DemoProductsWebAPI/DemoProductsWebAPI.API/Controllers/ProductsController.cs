using Microsoft.AspNetCore.Mvc;
using DemoProductsWebAPI.Common.DTOs;
using Microsoft.AspNetCore.Mvc;
using DemoProductsWebAPI.Common.Interfaces;
using MediatR;

namespace DemoProductsWebAPI.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    /// <summary>
    /// Controller that manages product resources.
    /// </summary>
    [Produces("application/json")]
    public class ProductsController(IMediator mediator) : BaseController
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        /// <summary>
        /// Gets all products.
        /// </summary>
        /// <returns>200 OK with the list of products.</returns>
        [HttpGet]
        [Microsoft.AspNetCore.OutputCaching.OutputCache]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            var list = await _mediator.Send(new Application.Products.Queries.GetAllProductsQuery()).ConfigureAwait(false);
            return ApiResponse(list);
        }

        /// <summary>
        /// Gets a product by identifier.
        /// </summary>
        /// <param name="id">The product identifier.</param>
        /// <returns>200 OK with the product when found; 404 NotFound otherwise.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _mediator.Send(new Application.Products.Queries.GetProductByIdQuery(id)).ConfigureAwait(false);
            return ApiResponse(product);
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="dto">Product data transfer object containing details to create.</param>
        /// <returns>201 Created with the created product.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(ProductDto dto)
        {
            var created = await _mediator.Send(new Application.Products.Commands.CreateProductCommand(dto)).ConfigureAwait(false);
            return ApiCreated(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="id">The id of the product to update.</param>
        /// <param name="dto">Updated product data.</param>
        /// <returns>204 NoContent on success, 400 BadRequest for invalid input, or 404 NotFound if the product does not exist.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, ProductDto dto)
        {
            if (id != dto.Id) return ApiBadRequest();
            var updated = await _mediator.Send(new Application.Products.Commands.UpdateProductCommand(dto)).ConfigureAwait(false);
            if (!updated) return NotFound();
            return ApiNoContent();
        }

        /// <summary>
        /// Deletes a product by id.
        /// </summary>
        /// <param name="id">The id of the product to delete.</param>
        /// <returns>204 NoContent on success or 404 NotFound if not found.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _mediator.Send(new Application.Products.Commands.DeleteProductCommand(id)).ConfigureAwait(false);
            if (!deleted) return NotFound();
            return ApiNoContent();
        }
    }
}
