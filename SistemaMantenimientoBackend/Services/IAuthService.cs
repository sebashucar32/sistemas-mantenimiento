using SistemaMantenimientoBackend.Models.Requests;
using SistemaMantenimientoBackend.Models.Responses;

namespace SistemaMantenimientoBackend.Services;

public interface IAuthService
{
    Task<LoginResponse?> IniciarSesionAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<UsuarioResponse?> ObtenerUsuarioAutenticadoAsync(int usuarioId, CancellationToken cancellationToken = default);
}
