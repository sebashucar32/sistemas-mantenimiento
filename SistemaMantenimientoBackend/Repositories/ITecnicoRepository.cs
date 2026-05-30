using SistemaMantenimientoBackend.Models;

namespace SistemaMantenimientoBackend.Repositories;

public interface ITecnicoRepository
{
    Task<bool> ExisteAsync(int id, CancellationToken cancellationToken = default);

    Task<Tecnico?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Tecnico>> ListarAsync(bool? activo = null, CancellationToken cancellationToken = default);

    Task<int> CrearAsync(Tecnico tecnico, CancellationToken cancellationToken = default);

    Task<bool> ActualizarAsync(Tecnico tecnico, CancellationToken cancellationToken = default);

    Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
}
