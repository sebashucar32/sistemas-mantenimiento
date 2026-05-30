using SistemaMantenimientoBackend.Models.Requests;
using SistemaMantenimientoBackend.Models.Responses;

namespace SistemaMantenimientoBackend.Services;

public interface ITecnicoService
{
    Task<(TecnicoResponse? Tecnico, string? Error)> CrearAsync(
        CrearTecnicoRequest request,
        CancellationToken cancellationToken = default);

    Task<TecnicoResponse?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TecnicoResponse>> ListarAsync(
        bool? activo = null,
        CancellationToken cancellationToken = default);

    Task<(TecnicoResponse? Tecnico, string? Error)> ActualizarAsync(
        int id,
        ActualizarTecnicoRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
}
