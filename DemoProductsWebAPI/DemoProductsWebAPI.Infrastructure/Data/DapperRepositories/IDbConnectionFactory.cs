using System.Data;

namespace DemoProductsWebAPI.Infrastructure.Data.DapperRepositories
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
