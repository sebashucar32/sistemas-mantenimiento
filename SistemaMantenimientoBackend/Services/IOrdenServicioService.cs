using SistemaMantenimientoBackend.Models.Requests;
using SistemaMantenimientoBackend.Models.Responses;

namespace SistemaMantenimientoBackend.Services;

public interface IOrdenServicioService
{
    Task<(OrdenServicioResponse? Orden, string? Error)> CrearAsync(
        CrearOrdenServicioRequest request,
        CancellationToken cancellationToken = default);

    Task<OrdenServicioResponse?> ObtenerPorIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OrdenServicioResponse>> ListarAsync(
        ListarOrdenesServicioFiltros filtros,
        CancellationToken cancellationToken = default);

    Task<(OrdenServicioResponse? Orden, string? Error)> ActualizarAsync(
        int id,
        ActualizarOrdenServicioRequest request,
        CancellationToken cancellationToken = default);

    Task<(OrdenServicioResponse? Orden, string? Error)> CambiarEstadoAsync(
        int id,
        CambiarEstadoOrdenServicioRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
}
