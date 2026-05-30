using Dapper;
using SistemaMantenimientoBackend.Data;
using SistemaMantenimientoBackend.Models;

namespace SistemaMantenimientoBackend.Repositories;

public class ClienteRepository(IDbConnectionFactory connectionFactory) : IClienteRepository
{
    private const string SelectColumns = """
        id AS Id,
        nombre AS Nombre,
        documento_identidad AS DocumentoIdentidad,
        direccion AS Direccion,
        telefono AS Telefono,
        activo AS Activo
        """;

    public async Task<bool> ExisteAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT 1
            FROM clientes
            WHERE id = @Id AND activo = TRUE
            """;

        using var connection = connectionFactory.CreateConnection();
        var resultado = await connection.QuerySingleOrDefaultAsync<int?>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return resultado.HasValue;
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT {SelectColumns}
            FROM clientes
            WHERE id = @Id
            """;

        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Cliente>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<Cliente>> ListarAsync(
        bool? activo = null,
        CancellationToken cancellationToken = default)
    {
        var condiciones = new List<string>();
        var parametros = new DynamicParameters();

        if (activo.HasValue)
        {
            condiciones.Add("activo = @Activo");
            parametros.Add("Activo", activo.Value);
        }

        var where = condiciones.Count > 0
            ? "WHERE " + string.Join(" AND ", condiciones)
            : string.Empty;

        var sql = $"""
            SELECT {SelectColumns}
            FROM clientes
            {where}
            ORDER BY nombre
            """;

        using var connection = connectionFactory.CreateConnection();
        var resultados = await connection.QueryAsync<Cliente>(
            new CommandDefinition(sql, parametros, cancellationToken: cancellationToken));

        return resultados.ToList();
    }

    public async Task<bool> ExisteDocumentoAsync(
        string documentoIdentidad,
        int? excluirId = null,
        CancellationToken cancellationToken = default)
    {
        var condiciones = new List<string> { "documento_identidad = @DocumentoIdentidad" };
        var parametros = new DynamicParameters();
        parametros.Add("DocumentoIdentidad", documentoIdentidad);

        if (excluirId.HasValue)
        {
            condiciones.Add("id <> @ExcluirId");
            parametros.Add("ExcluirId", excluirId.Value);
        }

        var sql = $"""
            SELECT 1
            FROM clientes
            WHERE {string.Join(" AND ", condiciones)}
            """;

        using var connection = connectionFactory.CreateConnection();
        var resultado = await connection.QuerySingleOrDefaultAsync<int?>(
            new CommandDefinition(sql, parametros, cancellationToken: cancellationToken));

        return resultado.HasValue;
    }

    public async Task<int> CrearAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO clientes (nombre, documento_identidad, direccion, telefono, activo)
            VALUES (@Nombre, @DocumentoIdentidad, @Direccion, @Telefono, @Activo)
            RETURNING id
            """;

        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                sql,
                new
                {
                    cliente.Nombre,
                    cliente.DocumentoIdentidad,
                    cliente.Direccion,
                    cliente.Telefono,
                    cliente.Activo
                },
                cancellationToken: cancellationToken));
    }

    public async Task<bool> ActualizarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE clientes
            SET nombre = @Nombre,
                documento_identidad = @DocumentoIdentidad,
                direccion = @Direccion,
                telefono = @Telefono,
                activo = @Activo
            WHERE id = @Id
            """;

        using var connection = connectionFactory.CreateConnection();
        var filasAfectadas = await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    cliente.Id,
                    cliente.Nombre,
                    cliente.DocumentoIdentidad,
                    cliente.Direccion,
                    cliente.Telefono,
                    cliente.Activo
                },
                cancellationToken: cancellationToken));

        return filasAfectadas > 0;
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE clientes
            SET activo = FALSE
            WHERE id = @Id AND activo = TRUE
            """;

        using var connection = connectionFactory.CreateConnection();
        var filasAfectadas = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return filasAfectadas > 0;
    }
}
