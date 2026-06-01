using Moq;
using SistemaMantenimientoBackend.Models;
using SistemaMantenimientoBackend.Models.Requests;
using SistemaMantenimientoBackend.Repositories;
using SistemaMantenimientoBackend.Services;

namespace SistemaMantenimientoBackend.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _service = new AuthService(
            _usuarioRepositoryMock.Object,
            _passwordHasherMock.Object,
            _tokenServiceMock.Object);
    }

    [Fact]
    public async Task IniciarSesionAsync_RetornaNull_CuandoUsuarioNoExiste()
    {
        var request = new LoginRequest { NombreUsuario = "admin", Contrasena = "Admin123!" };

        _usuarioRepositoryMock
            .Setup(r => r.ObtenerPorNombreUsuarioAsync("admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        var result = await _service.IniciarSesionAsync(request, CancellationToken.None);

        Assert.Null(result);
        _passwordHasherMock.Verify(h => h.Verificar(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task IniciarSesionAsync_RetornaNull_CuandoUsuarioEstaInactivo()
    {
        var request = new LoginRequest { NombreUsuario = "admin", Contrasena = "Admin123!" };
        var usuario = new Usuario { Id = 1, NombreUsuario = "admin", Activo = false };

        _usuarioRepositoryMock
            .Setup(r => r.ObtenerPorNombreUsuarioAsync("admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        var result = await _service.IniciarSesionAsync(request, CancellationToken.None);

        Assert.Null(result);
        _passwordHasherMock.Verify(h => h.Verificar(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task IniciarSesionAsync_RetornaNull_CuandoContrasenaEsIncorrecta()
    {
        var request = new LoginRequest { NombreUsuario = "admin", Contrasena = "incorrecta" };
        var usuario = new Usuario
        {
            Id = 1,
            NombreUsuario = "admin",
            HashContrasena = "hash",
            Activo = true
        };

        _usuarioRepositoryMock
            .Setup(r => r.ObtenerPorNombreUsuarioAsync("admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        _passwordHasherMock
            .Setup(h => h.Verificar("incorrecta", "hash"))
            .Returns(false);

        var result = await _service.IniciarSesionAsync(request, CancellationToken.None);

        Assert.Null(result);
        _tokenServiceMock.Verify(t => t.GenerarToken(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task IniciarSesionAsync_RetornaLoginResponse_CuandoCredencialesSonValidas()
    {
        var request = new LoginRequest { NombreUsuario = "  admin  ", Contrasena = "Admin123!" };
        var usuario = new Usuario
        {
            Id = 1,
            NombreUsuario = "admin",
            HashContrasena = "hash",
            NombreCompleto = "Administrador",
            CorreoElectronico = "admin@test.com",
            Activo = true
        };
        var expiraEn = DateTime.UtcNow.AddHours(1);

        _usuarioRepositoryMock
            .Setup(r => r.ObtenerPorNombreUsuarioAsync("admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        _passwordHasherMock
            .Setup(h => h.Verificar("Admin123!", "hash"))
            .Returns(true);

        _tokenServiceMock
            .Setup(t => t.GenerarToken(usuario))
            .Returns(("jwt-token", expiraEn));

        var result = await _service.IniciarSesionAsync(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("jwt-token", result.Token);
        Assert.Equal(expiraEn, result.ExpiraEn);
        Assert.Equal(1, result.Usuario.Id);
        Assert.Equal("admin", result.Usuario.NombreUsuario);
        Assert.Equal("Administrador", result.Usuario.NombreCompleto);
        Assert.Equal("admin@test.com", result.Usuario.CorreoElectronico);
    }

    [Fact]
    public async Task ObtenerUsuarioAutenticadoAsync_RetornaNull_CuandoUsuarioNoExiste()
    {
        _usuarioRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        var result = await _service.ObtenerUsuarioAutenticadoAsync(99, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task ObtenerUsuarioAutenticadoAsync_RetornaNull_CuandoUsuarioEstaInactivo()
    {
        _usuarioRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Usuario { Id = 1, Activo = false });

        var result = await _service.ObtenerUsuarioAutenticadoAsync(1, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task ObtenerUsuarioAutenticadoAsync_RetornaUsuarioResponse_CuandoUsuarioExiste()
    {
        var usuario = new Usuario
        {
            Id = 5,
            NombreUsuario = "operador",
            NombreCompleto = "Operador Sistema",
            CorreoElectronico = "operador@test.com",
            Activo = true
        };

        _usuarioRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        var result = await _service.ObtenerUsuarioAutenticadoAsync(5, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(5, result.Id);
        Assert.Equal("operador", result.NombreUsuario);
        Assert.Equal("Operador Sistema", result.NombreCompleto);
        Assert.Equal("operador@test.com", result.CorreoElectronico);
    }
}
