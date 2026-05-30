using SistemaMantenimientoBackend.Models;
using SistemaMantenimientoBackend.Models.Requests;
using SistemaMantenimientoBackend.Models.Responses;
using SistemaMantenimientoBackend.Repositories;

namespace SistemaMantenimientoBackend.Services;

public class AuthService(
    IUsuarioRepository usuarioRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : IAuthService
{
    public async Task<LoginResponse?> IniciarSesionAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuario = await usuarioRepository.ObtenerPorNombreUsuarioAsync(
            request.NombreUsuario.Trim(),
            cancellationToken);

        if (usuario is null || !usuario.Activo)
        {
            return null;
        }

        if (!passwordHasher.Verificar(request.Contrasena, usuario.HashContrasena))
        {
            return null;
        }

        var (token, expiraEn) = tokenService.GenerarToken(usuario);

        return new LoginResponse
        {
            Token = token,
            ExpiraEn = expiraEn,
            Usuario = MapearUsuario(usuario)
        };
    }

    public async Task<UsuarioResponse?> ObtenerUsuarioAutenticadoAsync(
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        var usuario = await usuarioRepository.ObtenerPorIdAsync(usuarioId, cancellationToken);

        if (usuario is null || !usuario.Activo)
        {
            return null;
        }

        return MapearUsuario(usuario);
    }

    private static UsuarioResponse MapearUsuario(Usuario usuario) => new()
    {
        Id = usuario.Id,
        NombreUsuario = usuario.NombreUsuario,
        NombreCompleto = usuario.NombreCompleto,
        CorreoElectronico = usuario.CorreoElectronico
    };
}
