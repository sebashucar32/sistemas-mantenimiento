using Dapper;
using SistemaMantenimientoBackend.Data;
using SistemaMantenimientoBackend.Models;

namespace SistemaMantenimientoBackend.Repositories;

public class UsuarioRepository(IDbConnectionFactory connectionFactory) : IUsuarioRepository
{
    private const string SelectColumns = """
        id AS Id,
        nombre_usuario AS NombreUsuario,
        hash_contrasena AS HashContrasena,
        nombre_completo AS NombreCompleto,
        correo_electronico AS CorreoElectronico,
        activo AS Activo
        """;

    public async Task<Usuario?> ObtenerPorNombreUsuarioAsync(
        string nombreUsuario,
        CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            SELECT {SelectColumns}
            FROM usuarios
            WHERE nombre_usuario = @NombreUsuario
            """;

        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Usuario>(
            new CommandDefinition(sql, new { NombreUsuario = nombreUsuario }, cancellationToken: cancellationToken));
    }

    public async Task<Usuario?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            SELECT {SelectColumns}
            FROM usuarios
            WHERE id = @Id
            """;

        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Usuario>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }
}
