using Dapper;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoProductsWebAPI.Infrastructure.Data.Read
{
    public interface IDapperExecutor
    {
        Task<IEnumerable<T>> QueryAsync<T>(IDbConnection connection, CommandDefinition command);
        Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection connection, CommandDefinition command);
    }
}
