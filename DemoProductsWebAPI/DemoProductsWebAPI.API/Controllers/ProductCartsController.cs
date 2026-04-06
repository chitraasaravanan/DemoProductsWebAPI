using Microsoft.AspNetCore.Mvc;
using DemoProductsWebAPI.Application.DTOs;
using MediatR;

namespace DemoProductsWebAPI.API.Controllers
{    
    public class ProductCartsController(IMediator mediator) : BaseController
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        /// <summary>
        /// Gets all product carts.
        /// </summary>
        [HttpGet]
        [Microsoft.AspNetCore.OutputCaching.OutputCache]
        public async Task<IActionResult> Get()
        {
            var list = await _mediator.Send(new Application.ProductCarts.Queries.GetAllProductCartsQuery()).ConfigureAwait(false);
            return ApiResponse(list);
        }

        /// <summary>
        /// Gets a product cart by id.
        /// </summary>
        [HttpGet("{id}")]
        [Microsoft.AspNetCore.OutputCaching.OutputCache]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _mediator.Send(new Application.ProductCarts.Queries.GetProductCartByIdQuery(id)).ConfigureAwait(false);
            return ApiResponse(item);
        }

        /// <summary>
        /// Creates a product cart.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(ProductCartDto dto)
        {
            var created = await _mediator.Send(new Application.ProductCarts.Commands.CreateProductCartCommand(dto)).ConfigureAwait(false);
            return ApiCreated(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates a product cart.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductCartDto dto)
        {
            if (id != dto.Id) return ApiBadRequest();
            var updated = await _mediator.Send(new Application.ProductCarts.Commands.UpdateProductCartCommand(dto)).ConfigureAwait(false);
            if (!updated) return NotFound();
            return ApiNoContent();
        }

        /// <summary>
        /// Deletes a product cart.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _mediator.Send(new Application.ProductCarts.Commands.DeleteProductCartCommand(id)).ConfigureAwait(false);
            if (!deleted) return NotFound();
            return ApiNoContent();
        }
    }
}
