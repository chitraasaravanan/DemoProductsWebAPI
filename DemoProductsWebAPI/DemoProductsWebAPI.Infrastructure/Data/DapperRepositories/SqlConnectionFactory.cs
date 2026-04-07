using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DemoProductsWebAPI.Infrastructure.Data.DapperRepositories
{
    public class SqlConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
