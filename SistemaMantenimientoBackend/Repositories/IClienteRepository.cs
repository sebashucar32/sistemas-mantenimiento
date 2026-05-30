using SistemaMantenimientoBackend.Models;

namespace SistemaMantenimientoBackend.Repositories;

public interface IClienteRepository
{
    Task<bool> ExisteAsync(int id, CancellationToken cancellationToken = default);

    Task<Cliente?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Cliente>> ListarAsync(bool? activo = null, CancellationToken cancellationToken = default);

    Task<bool> ExisteDocumentoAsync(
        string documentoIdentidad,
        int? excluirId = null,
        CancellationToken cancellationToken = default);

    Task<int> CrearAsync(Cliente cliente, CancellationToken cancellationToken = default);

    Task<bool> ActualizarAsync(Cliente cliente, CancellationToken cancellationToken = default);

    Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
}
