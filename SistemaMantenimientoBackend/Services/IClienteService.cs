using SistemaMantenimientoBackend.Models.Requests;
using SistemaMantenimientoBackend.Models.Responses;

namespace SistemaMantenimientoBackend.Services;

public interface IClienteService
{
    Task<(ClienteResponse? Cliente, string? Error)> CrearAsync(
        CrearClienteRequest request,
        CancellationToken cancellationToken = default);

    Task<ClienteResponse?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ClienteResponse>> ListarAsync(
        bool? activo = null,
        CancellationToken cancellationToken = default);

    Task<(ClienteResponse? Cliente, string? Error)> ActualizarAsync(
        int id,
        ActualizarClienteRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
}
