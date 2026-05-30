using SistemaMantenimientoBackend.Models;

namespace SistemaMantenimientoBackend.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken cancellationToken = default);
    Task<Usuario?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
}
