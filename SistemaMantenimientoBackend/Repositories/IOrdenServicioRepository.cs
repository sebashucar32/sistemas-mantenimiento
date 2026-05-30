using SistemaMantenimientoBackend.Models;
using SistemaMantenimientoBackend.Models.Requests;

namespace SistemaMantenimientoBackend.Repositories;

public interface IOrdenServicioRepository
{
    Task<OrdenServicio?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OrdenServicio>> ListarAsync(
        ListarOrdenesServicioFiltros filtros,
        CancellationToken cancellationToken = default);

    Task<int> CrearAsync(OrdenServicio orden, CancellationToken cancellationToken = default);

    Task<bool> ActualizarAsync(OrdenServicio orden, CancellationToken cancellationToken = default);

    Task<bool> ActualizarEstadoAsync(
        int id,
        string estado,
        CancellationToken cancellationToken = default);

    Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
}
