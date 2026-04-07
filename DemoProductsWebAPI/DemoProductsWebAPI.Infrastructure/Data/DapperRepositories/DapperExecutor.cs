using Dapper;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using System.Data;

namespace DemoProductsWebAPI.Infrastructure.Data.DapperRepositories
{
    public class DapperExecutor(ILogger<DapperExecutor> logger) : IDapperExecutor
    {
        private readonly ILogger<DapperExecutor> _logger = logger;
        private readonly AsyncRetryPolicy _retryPolicy = InitRetryPolicy(logger);
        private readonly AsyncCircuitBreakerPolicy _circuitBreaker = InitCircuitBreaker(logger);
        private static readonly System.Diagnostics.Metrics.Meter _meter = new("DemoProductsWebAPI.DapperExecutor", "1.0");
        private static readonly System.Diagnostics.Metrics.Counter<long> _retryCounter = _meter.CreateCounter<long>("dapper_retries");
        private static readonly System.Diagnostics.Metrics.Counter<long> _circuitOpenCounter = _meter.CreateCounter<long>("dapper_circuit_open");

        private static AsyncRetryPolicy InitRetryPolicy(ILogger<DapperExecutor> logger)
        {
            var jitter = new Random();
            return Policy.Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt =>
                {
                    var baseDelay = TimeSpan.FromMilliseconds(200 * Math.Pow(2, retryAttempt - 1));
                    var jitterDelay = TimeSpan.FromMilliseconds(jitter.Next(0, 100));
                    return baseDelay + jitterDelay;
                }, (ex, span, attempt, context) =>
                {
                    logger.LogWarning(ex, "Transient error during Dapper operation, attempt {Attempt} will retry after {Delay}", attempt, span);
                    _retryCounter.Add(1);
                });
        }

        private static AsyncCircuitBreakerPolicy InitCircuitBreaker(ILogger<DapperExecutor> logger)
        {
            return Policy.Handle<Exception>()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30), onBreak: (ex, ts) =>
                {
                    logger.LogWarning(ex, "Circuit breaker opened for {TimeSpan}", ts);
                    _circuitOpenCounter.Add(1);
                }, onReset: () => logger.LogInformation("Circuit breaker reset"));
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
            }, token == default ? CancellationToken.None : token));
        }

        public async Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection connection, CommandDefinition command)
        {
            var token = command.CancellationToken;
            return await _circuitBreaker.ExecuteAsync(async () => await _retryPolicy.ExecuteAsync(async ct =>
            {
                if (connection is System.Data.Common.DbConnection dbConn && dbConn.State != ConnectionState.Open)
                    await dbConn.OpenAsync(ct);

                return await connection.QuerySingleOrDefaultAsync<T>(command);
            }, token == default ? CancellationToken.None : token));
        }
    }
}
