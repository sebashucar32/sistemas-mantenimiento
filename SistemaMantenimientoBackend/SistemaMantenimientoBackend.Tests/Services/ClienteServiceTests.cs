using Moq;
using SistemaMantenimientoBackend.Models;
using SistemaMantenimientoBackend.Models.Requests;
using SistemaMantenimientoBackend.Repositories;
using SistemaMantenimientoBackend.Services;

namespace SistemaMantenimientoBackend.Tests.Services;

public class ClienteServiceTests
{
    private readonly Mock<IClienteRepository> _clienteRepositoryMock = new();
    private readonly ClienteService _service;

    public ClienteServiceTests()
    {
        _service = new ClienteService(_clienteRepositoryMock.Object);
    }

    [Fact]
    public async Task CrearAsync_RetornaError_CuandoDocumentoYaExiste()
    {
        var request = new CrearClienteRequest
        {
            Nombre = "Juan Pérez",
            DocumentoIdentidad = " 12345678 ",
            Direccion = "Calle 1",
            Telefono = "3001234567"
        };

        _clienteRepositoryMock
            .Setup(r => r.ExisteDocumentoAsync("12345678", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var (cliente, error) = await _service.CrearAsync(request, CancellationToken.None);

        Assert.Null(cliente);
        Assert.Equal("Ya existe un cliente con el mismo documento de identidad.", error);
        _clienteRepositoryMock.Verify(r => r.CrearAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CrearAsync_RetornaCliente_CuandoDatosSonValidos()
    {
        var request = new CrearClienteRequest
        {
            Nombre = "  Juan Pérez  ",
            DocumentoIdentidad = " 12345678 ",
            Direccion = " Calle 1 ",
            Telefono = " 3001234567 "
        };
        var creado = new Cliente
        {
            Id = 1,
            Nombre = "Juan Pérez",
            DocumentoIdentidad = "12345678",
            Direccion = "Calle 1",
            Telefono = "3001234567",
            Activo = true
        };

        _clienteRepositoryMock
            .Setup(r => r.ExisteDocumentoAsync("12345678", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _clienteRepositoryMock
            .Setup(r => r.CrearAsync(It.Is<Cliente>(c =>
                c.Nombre == "Juan Pérez" &&
                c.DocumentoIdentidad == "12345678" &&
                c.Direccion == "Calle 1" &&
                c.Telefono == "3001234567" &&
                c.Activo), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _clienteRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(creado);

        var (cliente, error) = await _service.CrearAsync(request, CancellationToken.None);

        Assert.Null(error);
        Assert.NotNull(cliente);
        Assert.Equal(1, cliente.Id);
        Assert.Equal("Juan Pérez", cliente.Nombre);
        Assert.True(cliente.Activo);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_RetornaNull_CuandoClienteNoExiste()
    {
        _clienteRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cliente?)null);

        var result = await _service.ObtenerPorIdAsync(99, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_RetornaCliente_CuandoExiste()
    {
        var cliente = new Cliente { Id = 1, Nombre = "Juan Pérez", Activo = true };

        _clienteRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        var result = await _service.ObtenerPorIdAsync(1, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Juan Pérez", result.Nombre);
    }

    [Fact]
    public async Task ListarAsync_RetornaListaMapeada()
    {
        var clientes = new List<Cliente>
        {
            new() { Id = 1, Nombre = "Cliente A", Activo = true },
            new() { Id = 2, Nombre = "Cliente B", Activo = false }
        };

        _clienteRepositoryMock
            .Setup(r => r.ListarAsync(true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(clientes);

        var result = await _service.ListarAsync(true, CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Equal("Cliente A", result[0].Nombre);
        Assert.Equal("Cliente B", result[1].Nombre);
    }

    [Fact]
    public async Task ActualizarAsync_RetornaError_CuandoClienteNoExiste()
    {
        var request = new ActualizarClienteRequest { Nombre = "Nombre" };

        _clienteRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cliente?)null);

        var (cliente, error) = await _service.ActualizarAsync(99, request, CancellationToken.None);

        Assert.Null(cliente);
        Assert.Equal("Cliente no encontrado.", error);
    }

    [Fact]
    public async Task ActualizarAsync_RetornaError_CuandoDocumentoYaExiste()
    {
        var request = new ActualizarClienteRequest
        {
            Nombre = "Juan",
            DocumentoIdentidad = "99999999",
            Direccion = "Calle 2",
            Telefono = "3000000000",
            Activo = true
        };

        _clienteRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Cliente { Id = 1, Nombre = "Anterior" });

        _clienteRepositoryMock
            .Setup(r => r.ExisteDocumentoAsync("99999999", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var (cliente, error) = await _service.ActualizarAsync(1, request, CancellationToken.None);

        Assert.Null(cliente);
        Assert.Equal("Ya existe un cliente con el mismo documento de identidad.", error);
    }

    [Fact]
    public async Task ActualizarAsync_RetornaClienteActualizado_CuandoDatosSonValidos()
    {
        var existente = new Cliente
        {
            Id = 1,
            Nombre = "Anterior",
            DocumentoIdentidad = "11111111",
            Direccion = "Calle 0",
            Telefono = "3001111111",
            Activo = true
        };
        var request = new ActualizarClienteRequest
        {
            Nombre = "  Juan Actualizado  ",
            DocumentoIdentidad = " 22222222 ",
            Direccion = " Calle 2 ",
            Telefono = " 3002222222 ",
            Activo = false
        };
        var actualizado = new Cliente
        {
            Id = 1,
            Nombre = "Juan Actualizado",
            DocumentoIdentidad = "22222222",
            Direccion = "Calle 2",
            Telefono = "3002222222",
            Activo = false
        };

        _clienteRepositoryMock
            .SetupSequence(r => r.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existente)
            .ReturnsAsync(actualizado);

        _clienteRepositoryMock
            .Setup(r => r.ExisteDocumentoAsync("22222222", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _clienteRepositoryMock
            .Setup(r => r.ActualizarAsync(It.Is<Cliente>(c =>
                c.Nombre == "Juan Actualizado" &&
                c.DocumentoIdentidad == "22222222" &&
                c.Activo == false), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var (cliente, error) = await _service.ActualizarAsync(1, request, CancellationToken.None);

        Assert.Null(error);
        Assert.NotNull(cliente);
        Assert.Equal("Juan Actualizado", cliente.Nombre);
        Assert.False(cliente.Activo);
    }

    [Fact]
    public async Task EliminarAsync_RetornaResultadoDelRepositorio()
    {
        _clienteRepositoryMock
            .Setup(r => r.EliminarAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.EliminarAsync(1, CancellationToken.None);

        Assert.True(result);
    }
}
