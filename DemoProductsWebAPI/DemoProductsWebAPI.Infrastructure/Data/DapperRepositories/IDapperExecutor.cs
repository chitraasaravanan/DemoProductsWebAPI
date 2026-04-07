using Dapper;
using System.Data;

namespace DemoProductsWebAPI.Infrastructure.Data.DapperRepositories
{
    public interface IDapperExecutor
    {
        Task<IEnumerable<T>> QueryAsync<T>(IDbConnection connection, CommandDefinition command);
        Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection connection, CommandDefinition command);
    }
}
