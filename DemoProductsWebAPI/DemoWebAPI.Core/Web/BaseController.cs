using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoWebAPI.Core.Web
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    /// <summary>
    /// Base Controller.
    /// </summary>
    [Produces("application/json")]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// Standardized API response that returns 200 OK with the result or 404 NotFound when result is null.
        /// </summary>
        /// <typeparam name="T">Type of the result.</typeparam>
        /// <param name="result">The result to return.</param>
        /// <returns>200 OK with the result or 404 NotFound.</returns>
        protected IActionResult ApiResponse<T>(T? result)
        {
            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Creates a 201 Created response using the specified action and route values.
        /// </summary>
        /// <param name="actionName">The action name for CreatedAtAction.</param>
        /// <param name="routeValues">Route values used to generate the resource URL.</param>
        /// <param name="value">The response body value.</param>
        /// <returns>201 Created response.</returns>
        protected IActionResult ApiCreated(string actionName, object? routeValues, object? value)
        {
            return CreatedAtAction(actionName, routeValues ?? new { }, value);
        }

        /// <summary>
        /// Returns a 204 NoContent response.
        /// </summary>
        /// <returns>204 NoContent.</returns>
        protected IActionResult ApiNoContent() => NoContent();

        /// <summary>
        /// Returns a 400 BadRequest response.
        /// </summary>
        /// <returns>400 BadRequest.</returns>
        protected IActionResult ApiBadRequest() => BadRequest();
    }
}
