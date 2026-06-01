using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using SistemaMantenimientoBackend.Configuration;
using SistemaMantenimientoBackend.Models;
using SistemaMantenimientoBackend.Services;

namespace SistemaMantenimientoBackend.Tests.Services;

public class TokenServiceTests
{
    private static readonly JwtSettings JwtSettings = new()
    {
        Key = "ClaveSecretaDePruebaMinimo32Caracteres!",
        Issuer = "SistemaMantenimiento",
        Audience = "SistemaMantenimientoClientes",
        ExpirationMinutes = 60
    };

    private readonly TokenService _service = new(Options.Create(JwtSettings));

    [Fact]
    public void GenerarToken_RetornaJwtValido_ConClaimsEsperados()
    {
        var usuario = new Usuario
        {
            Id = 42,
            NombreUsuario = "admin",
            NombreCompleto = "Administrador",
            CorreoElectronico = "admin@test.com"
        };

        var antes = DateTime.UtcNow;
        var (token, expiraEn) = _service.GenerarToken(usuario);
        var despues = DateTime.UtcNow;

        Assert.False(string.IsNullOrWhiteSpace(token));

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        Assert.Equal(JwtSettings.Issuer, jwt.Issuer);
        Assert.Equal(JwtSettings.Audience, jwt.Audiences.Single());
        Assert.Equal("42", jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal("admin", jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value);
        Assert.Equal("Administrador", jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Name).Value);
        Assert.Equal("admin@test.com", jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
        Assert.NotNull(jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti));

        var expiracionEsperadaMin = antes.AddMinutes(JwtSettings.ExpirationMinutes);
        var expiracionEsperadaMax = despues.AddMinutes(JwtSettings.ExpirationMinutes);
        Assert.InRange(expiraEn, expiracionEsperadaMin, expiracionEsperadaMax);
    }

    [Fact]
    public void GenerarToken_NoIncluyeClaimEmail_CuandoCorreoEsNuloOVacio()
    {
        var usuario = new Usuario
        {
            Id = 1,
            NombreUsuario = "admin",
            NombreCompleto = "Administrador",
            CorreoElectronico = "   "
        };

        var (token, _) = _service.GenerarToken(usuario);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Null(jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email));
    }
}
