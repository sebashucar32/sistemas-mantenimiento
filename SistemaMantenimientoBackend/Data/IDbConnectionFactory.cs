using System.Data;

namespace SistemaMantenimientoBackend.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
