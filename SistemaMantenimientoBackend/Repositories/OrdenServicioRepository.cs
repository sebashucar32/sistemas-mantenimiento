using Dapper;
using SistemaMantenimientoBackend.Data;
using SistemaMantenimientoBackend.Models;
using SistemaMantenimientoBackend.Models.Enums;
using SistemaMantenimientoBackend.Models.Requests;

namespace SistemaMantenimientoBackend.Repositories;

public class OrdenServicioRepository(IDbConnectionFactory connectionFactory) : IOrdenServicioRepository
{
    private const string SelectColumns = """
        o.id AS Id,
        o.fecha_creacion AS FechaCreacion,
        o.estado AS Estado,
        o.descripcion AS Descripcion,
        o.tecnico_id AS TecnicoId,
        o.cliente_id AS ClienteId,
        t.nombre AS TecnicoNombre,
        t.especialidad AS TecnicoEspecialidad,
        c.nombre AS ClienteNombre,
        c.documento_identidad AS ClienteDocumentoIdentidad
        """;

    private const string FromJoin = """
        FROM ordenes_servicio o
        INNER JOIN tecnicos t ON t.id = o.tecnico_id
        INNER JOIN clientes c ON c.id = o.cliente_id
        """;

    public async Task<OrdenServicio?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT {SelectColumns}
            {FromJoin}
            WHERE o.id = @Id
            """;

        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<OrdenServicio>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<OrdenServicio>> ListarAsync(
        ListarOrdenesServicioFiltros filtros,
        CancellationToken cancellationToken = default)
    {
        var condiciones = new List<string>();
        var parametros = new DynamicParameters();

        if (filtros.Estado.HasValue)
        {
            condiciones.Add("o.estado = @Estado");
            parametros.Add("Estado", EstadoOrdenServicioMapper.AValorBaseDatos(filtros.Estado.Value));
        }

        if (filtros.ClienteId.HasValue)
        {
            condiciones.Add("o.cliente_id = @ClienteId");
            parametros.Add("ClienteId", filtros.ClienteId.Value);
        }

        if (filtros.TecnicoId.HasValue)
        {
            condiciones.Add("o.tecnico_id = @TecnicoId");
            parametros.Add("TecnicoId", filtros.TecnicoId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filtros.TecnicoBusqueda))
        {
            condiciones.Add("(t.nombre ILIKE @TecnicoBusqueda OR t.especialidad ILIKE @TecnicoBusqueda)");
            parametros.Add("TecnicoBusqueda", $"%{filtros.TecnicoBusqueda.Trim()}%");
        }

        if (!string.IsNullOrWhiteSpace(filtros.ClienteBusqueda))
        {
            condiciones.Add("(c.nombre ILIKE @ClienteBusqueda OR c.documento_identidad ILIKE @ClienteBusqueda)");
            parametros.Add("ClienteBusqueda", $"%{filtros.ClienteBusqueda.Trim()}%");
        }

        if (filtros.FechaCreacionDesde.HasValue)
        {
            condiciones.Add("o.fecha_creacion >= @FechaCreacionDesde");
            parametros.Add("FechaCreacionDesde", filtros.FechaCreacionDesde.Value);
        }

        if (filtros.FechaCreacionHasta.HasValue)
        {
            condiciones.Add("o.fecha_creacion <= @FechaCreacionHasta");
            parametros.Add("FechaCreacionHasta", filtros.FechaCreacionHasta.Value);
        }

        var where = condiciones.Count > 0
            ? "WHERE " + string.Join(" AND ", condiciones)
            : string.Empty;

        var sql = $"""
            SELECT {SelectColumns}
            {FromJoin}
            {where}
            ORDER BY o.fecha_creacion DESC
            """;

        using var connection = connectionFactory.CreateConnection();
        var resultados = await connection.QueryAsync<OrdenServicio>(
            new CommandDefinition(sql, parametros, cancellationToken: cancellationToken));

        return resultados.ToList();
    }

    public async Task<int> CrearAsync(OrdenServicio orden, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO ordenes_servicio (estado, descripcion, tecnico_id, cliente_id)
            VALUES (@Estado, @Descripcion, @TecnicoId, @ClienteId)
            RETURNING id
            """;

        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                sql,
                new
                {
                    orden.Estado,
                    orden.Descripcion,
                    orden.TecnicoId,
                    orden.ClienteId
                },
                cancellationToken: cancellationToken));
    }

    public async Task<bool> ActualizarAsync(OrdenServicio orden, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE ordenes_servicio
            SET estado = @Estado,
                descripcion = @Descripcion,
                tecnico_id = @TecnicoId,
                cliente_id = @ClienteId
            WHERE id = @Id
            """;

        using var connection = connectionFactory.CreateConnection();
        var filasAfectadas = await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    orden.Id,
                    orden.Estado,
                    orden.Descripcion,
                    orden.TecnicoId,
                    orden.ClienteId
                },
                cancellationToken: cancellationToken));

        return filasAfectadas > 0;
    }

    public async Task<bool> ActualizarEstadoAsync(
        int id,
        string estado,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE ordenes_servicio
            SET estado = @Estado
            WHERE id = @Id
            """;

        using var connection = connectionFactory.CreateConnection();
        var filasAfectadas = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id, Estado = estado }, cancellationToken: cancellationToken));

        return filasAfectadas > 0;
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            DELETE FROM ordenes_servicio
            WHERE id = @Id
            """;

        using var connection = connectionFactory.CreateConnection();
        var filasAfectadas = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return filasAfectadas > 0;
    }
}
