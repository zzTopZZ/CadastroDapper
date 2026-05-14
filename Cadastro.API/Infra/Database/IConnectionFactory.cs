using System.Data;

namespace Cadastro.API.Infra.Database;

public interface IConnectionFactory
{
    IDbConnection CreateConnection();
}
