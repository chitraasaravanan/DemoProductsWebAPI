using Dapper;
using Polly;
using Polly.Retry;
using Polly.CircuitBreaker;
using System.Data;
using Microsoft.Extensions.Logging;

namespace DemoProductsWebAPI.Infrastructure.Data.Read
{
    public class DapperExecutor : IDapperExecutor
    {
        private readonly ILogger<DapperExecutor> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;

        private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
        private static readonly System.Diagnostics.Metrics.Meter _meter = new("DemoProductsWebAPI.DapperExecutor", "1.0");
        private static readonly System.Diagnostics.Metrics.Counter<long> _retryCounter = _meter.CreateCounter<long>("dapper_retries");
        private static readonly System.Diagnostics.Metrics.Counter<long> _circuitOpenCounter = _meter.CreateCounter<long>("dapper_circuit_open");

        public DapperExecutor(ILogger<DapperExecutor> logger)
        {
            _logger = logger;
            // Exponential backoff with jitter
            var jitter = new Random();
            _retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt =>
                {
                    var baseDelay = TimeSpan.FromMilliseconds(200 * Math.Pow(2, retryAttempt - 1));
                    var jitterDelay = TimeSpan.FromMilliseconds(jitter.Next(0, 100));
                    return baseDelay + jitterDelay;
                }, (ex, span, attempt, context) =>
                {
                    _logger.LogWarning(ex, "Transient error during Dapper operation, attempt {Attempt} will retry after {Delay}", attempt, span);
                    _retryCounter.Add(1);
                });

            _circuitBreaker = Policy.Handle<Exception>()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30), onBreak: (ex, ts) =>
                {
                    _logger.LogWarning(ex, "Circuit breaker opened for {TimeSpan}", ts);
                    _circuitOpenCounter.Add(1);
                }, onReset: () => _logger.LogInformation("Circuit breaker reset"));
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(IDbConnection connection, CommandDefinition command)
        {
            var token = command.CancellationToken;
            return await _circuitBreaker.ExecuteAsync(async () => await _retryPolicy.ExecuteAsync(async ct =>
            {
                // Ensure connection is open
                if (connection is System.Data.Common.DbConnection dbConn && dbConn.State != ConnectionState.Open)
                    await dbConn.OpenAsync(ct);

                return await connection.QueryAsync<T>(command);
            }, token == default ? System.Threading.CancellationToken.None : token));
        }

        public async Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection connection, CommandDefinition command)
        {
            var token = command.CancellationToken;
            return await _circuitBreaker.ExecuteAsync(async () => await _retryPolicy.ExecuteAsync(async ct =>
            {
                if (connection is System.Data.Common.DbConnection dbConn && dbConn.State != ConnectionState.Open)
                    await dbConn.OpenAsync(ct);

                return await connection.QuerySingleOrDefaultAsync<T>(command);
            }, token == default ? System.Threading.CancellationToken.None : token));
        }
    }
}
