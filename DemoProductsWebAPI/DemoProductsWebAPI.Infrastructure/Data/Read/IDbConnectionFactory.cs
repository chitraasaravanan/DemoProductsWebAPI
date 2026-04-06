using System.Data;

namespace DemoProductsWebAPI.Infrastructure.Data.Read
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
