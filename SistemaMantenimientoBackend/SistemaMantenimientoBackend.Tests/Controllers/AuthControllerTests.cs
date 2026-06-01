using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SistemaMantenimientoBackend.Controllers;
using SistemaMantenimientoBackend.Models.Requests;
using SistemaMantenimientoBackend.Models.Responses;
using SistemaMantenimientoBackend.Services;

namespace SistemaMantenimientoBackend.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock = new();

    [Fact]
    public async Task Login_RetornaOk_CuandoCredencialesSonValidas()
    {
        var request = new LoginRequest { NombreUsuario = "admin", Contrasena = "Admin123!" };
        var loginResponse = new LoginResponse
        {
            Token = "jwt-token",
            ExpiraEn = DateTime.UtcNow.AddHours(1),
            Usuario = new UsuarioResponse { Id = 1, NombreUsuario = "admin" }
        };

        _authServiceMock
            .Setup(s => s.IniciarSesionAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(loginResponse);

        var controller = new AuthController(_authServiceMock.Object);

        var result = await controller.Login(request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(loginResponse, ok.Value);
    }

    [Fact]
    public async Task Login_RetornaUnauthorized_CuandoCredencialesSonInvalidas()
    {
        var request = new LoginRequest { NombreUsuario = "admin", Contrasena = "incorrecta" };

        _authServiceMock
            .Setup(s => s.IniciarSesionAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((LoginResponse?)null);

        var controller = new AuthController(_authServiceMock.Object);

        var result = await controller.Login(request, CancellationToken.None);

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(401, unauthorized.StatusCode);
    }

    [Fact]
    public async Task ValidarSesion_RetornaOk_CuandoUsuarioExiste()
    {
        var usuario = new UsuarioResponse { Id = 5, NombreUsuario = "admin" };

        _authServiceMock
            .Setup(s => s.ObtenerUsuarioAutenticadoAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        var controller = CrearControllerConUsuario("5");

        var result = await controller.ValidarSesion(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(usuario, ok.Value);
    }

    [Fact]
    public async Task ValidarSesion_RetornaUnauthorized_CuandoClaimIdEsInvalido()
    {
        var controller = CrearControllerConUsuario("no-es-numero");

        var result = await controller.ValidarSesion(CancellationToken.None);

        Assert.IsType<UnauthorizedResult>(result);
        _authServiceMock.Verify(
            s => s.ObtenerUsuarioAutenticadoAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ValidarSesion_RetornaUnauthorized_CuandoNoHayClaimDeUsuario()
    {
        var controller = new AuthController(_authServiceMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var result = await controller.ValidarSesion(CancellationToken.None);

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task ValidarSesion_RetornaNotFound_CuandoUsuarioNoExiste()
    {
        _authServiceMock
            .Setup(s => s.ObtenerUsuarioAutenticadoAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UsuarioResponse?)null);

        var controller = CrearControllerConUsuario("3");

        var result = await controller.ValidarSesion(CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task ValidarSesion_UsaClaimNameIdentifier_CuandoSubNoEstaPresente()
    {
        var usuario = new UsuarioResponse { Id = 7, NombreUsuario = "operador" };

        _authServiceMock
            .Setup(s => s.ObtenerUsuarioAutenticadoAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        var claims = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, "7")
        ], "TestAuth"));

        var controller = new AuthController(_authServiceMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claims }
        };

        var result = await controller.ValidarSesion(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(usuario, ok.Value);
    }

    private AuthController CrearControllerConUsuario(string userId)
    {
        var claims = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(JwtRegisteredClaimNames.Sub, userId)
        ], "TestAuth"));

        var controller = new AuthController(_authServiceMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claims }
        };

        return controller;
    }
}
