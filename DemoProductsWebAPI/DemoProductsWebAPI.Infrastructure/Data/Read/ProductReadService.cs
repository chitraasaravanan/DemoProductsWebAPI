using Dapper;
using DemoProductsWebAPI.Common.DTOs;
using DemoProductsWebAPI.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DemoProductsWebAPI.Infrastructure.Data.Read
{
    public class ProductReadService(IDbConnectionFactory factory, ILogger<ProductReadService> logger, IDapperExecutor executor) : IProductReadService
    {
        private readonly IDbConnectionFactory _factory = factory;
        private readonly ILogger<ProductReadService> _logger = logger;
        private readonly IDapperExecutor _executor = executor;

        /// <summary>
        /// Get all products (read-only) using optimized Dapper query.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>Collection of <see cref="ProductDto"/>.</returns>
        public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var sql = "SELECT Id, ProductName, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn FROM Product";
            _logger.LogInformation("Executing Dapper query for GetAllAsync");
            using var conn = _factory.CreateConnection();
            await OpenIfNeededAsync(conn, cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
            var list = await _executor.QueryAsync<ProductDto>(conn, command).ConfigureAwait(false);
            return list;
        }

        /// <summary>
        /// Get product by identifier using optimized Dapper query.
        /// </summary>
        /// <param name="id">Product identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="ProductDto"/> when found; otherwise null.</returns>
        public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var sql = "SELECT Id, ProductName, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn FROM Product WHERE Id = @Id";
            using var conn = _factory.CreateConnection();
            await OpenIfNeededAsync(conn, cancellationToken).ConfigureAwait(false);
            var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
            var item = await _executor.QuerySingleOrDefaultAsync<ProductDto>(conn, command).ConfigureAwait(false);
            return item;
        }

        private static async Task OpenIfNeededAsync(IDbConnection connection, CancellationToken cancellationToken)
        {
            if (connection is System.Data.Common.DbConnection dbConn && dbConn.State != ConnectionState.Open)
            {
                await dbConn.OpenAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
