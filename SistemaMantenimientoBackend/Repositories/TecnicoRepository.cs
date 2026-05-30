using Dapper;
using SistemaMantenimientoBackend.Data;
using SistemaMantenimientoBackend.Models;

namespace SistemaMantenimientoBackend.Repositories;

public class TecnicoRepository(IDbConnectionFactory connectionFactory) : ITecnicoRepository
{
    private const string SelectColumns = """
        id AS Id,
        nombre AS Nombre,
        telefono AS Telefono,
        especialidad AS Especialidad,
        activo AS Activo
        """;

    public async Task<bool> ExisteAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT 1
            FROM tecnicos
            WHERE id = @Id AND activo = TRUE
            """;

        using var connection = connectionFactory.CreateConnection();
        var resultado = await connection.QuerySingleOrDefaultAsync<int?>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return resultado.HasValue;
    }

    public async Task<Tecnico?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT {SelectColumns}
            FROM tecnicos
            WHERE id = @Id
            """;

        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Tecnico>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<Tecnico>> ListarAsync(
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
            FROM tecnicos
            {where}
            ORDER BY nombre
            """;

        using var connection = connectionFactory.CreateConnection();
        var resultados = await connection.QueryAsync<Tecnico>(
            new CommandDefinition(sql, parametros, cancellationToken: cancellationToken));

        return resultados.ToList();
    }

    public async Task<int> CrearAsync(Tecnico tecnico, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO tecnicos (nombre, telefono, especialidad, activo)
            VALUES (@Nombre, @Telefono, @Especialidad, @Activo)
            RETURNING id
            """;

        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                sql,
                new
                {
                    tecnico.Nombre,
                    tecnico.Telefono,
                    tecnico.Especialidad,
                    tecnico.Activo
                },
                cancellationToken: cancellationToken));
    }

    public async Task<bool> ActualizarAsync(Tecnico tecnico, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE tecnicos
            SET nombre = @Nombre,
                telefono = @Telefono,
                especialidad = @Especialidad,
                activo = @Activo
            WHERE id = @Id
            """;

        using var connection = connectionFactory.CreateConnection();
        var filasAfectadas = await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    tecnico.Id,
                    tecnico.Nombre,
                    tecnico.Telefono,
                    tecnico.Especialidad,
                    tecnico.Activo
                },
                cancellationToken: cancellationToken));

        return filasAfectadas > 0;
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE tecnicos
            SET activo = FALSE
            WHERE id = @Id AND activo = TRUE
            """;

        using var connection = connectionFactory.CreateConnection();
        var filasAfectadas = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return filasAfectadas > 0;
    }
}
