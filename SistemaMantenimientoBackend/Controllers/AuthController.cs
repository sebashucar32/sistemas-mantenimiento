using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaMantenimientoBackend.Models.Requests;
using SistemaMantenimientoBackend.Models.Responses;
using SistemaMantenimientoBackend.Services;

namespace SistemaMantenimientoBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var resultado = await authService.IniciarSesionAsync(request, cancellationToken);

        if (resultado is null)
        {
            return Unauthorized(new { mensaje = "Credenciales inválidas." });
        }

        return Ok(resultado);
    }

    [HttpGet("sesion")]
    [Authorize]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ValidarSesion(CancellationToken cancellationToken)
    {
        var claimId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(claimId, out var usuarioId))
        {
            return Unauthorized();
        }

        var usuario = await authService.ObtenerUsuarioAutenticadoAsync(usuarioId, cancellationToken);

        if (usuario is null)
        {
            return NotFound(new { mensaje = "Usuario no encontrado o inactivo." });
        }

        return Ok(usuario);
    }
}
