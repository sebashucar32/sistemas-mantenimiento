using System.Data;
using Npgsql;

namespace SistemaMantenimientoBackend.Data;

public class DbConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
{
    private readonly string _connectionString =
        configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' no está configurada.");

    public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
}
