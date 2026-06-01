using Microsoft.AspNetCore.Mvc;
using Moq;
using SistemaMantenimientoBackend.Controllers;
using SistemaMantenimientoBackend.Models.Requests;
using SistemaMantenimientoBackend.Models.Responses;
using SistemaMantenimientoBackend.Services;

namespace SistemaMantenimientoBackend.Tests.Controllers;

public class ClientesControllerTests
{
    private readonly Mock<IClienteService> _clienteServiceMock = new();
    private readonly ClientesController _controller;

    public ClientesControllerTests()
    {
        _controller = new ClientesController(_clienteServiceMock.Object);
    }

    [Fact]
    public async Task Crear_RetornaCreated_CuandoServicioEsExitoso()
    {
        var request = new CrearClienteRequest
        {
            Nombre = "Juan Pérez",
            DocumentoIdentidad = "12345678",
            Direccion = "Calle 1",
            Telefono = "3001234567"
        };
        var cliente = new ClienteResponse
        {
            Id = 1,
            Nombre = request.Nombre,
            DocumentoIdentidad = request.DocumentoIdentidad,
            Direccion = request.Direccion,
            Telefono = request.Telefono,
            Activo = true
        };

        _clienteServiceMock
            .Setup(s => s.CrearAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((cliente, null));

        var result = await _controller.Crear(request, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, created.StatusCode);
        Assert.Equal(cliente, created.Value);
        Assert.Equal(nameof(ClientesController.ObtenerPorId), created.ActionName);
    }

    [Fact]
    public async Task Crear_RetornaBadRequest_CuandoServicioRetornaError()
    {
        var request = new CrearClienteRequest { Nombre = "Duplicado" };

        _clienteServiceMock
            .Setup(s => s.CrearAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((null, "El documento de identidad ya existe."));

        var result = await _controller.Crear(request, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public async Task ObtenerPorId_RetornaOk_CuandoClienteExiste()
    {
        var cliente = new ClienteResponse { Id = 1, Nombre = "Juan Pérez" };

        _clienteServiceMock
            .Setup(s => s.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        var result = await _controller.ObtenerPorId(1, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(cliente, ok.Value);
    }

    [Fact]
    public async Task ObtenerPorId_RetornaNotFound_CuandoClienteNoExiste()
    {
        _clienteServiceMock
            .Setup(s => s.ObtenerPorIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClienteResponse?)null);

        var result = await _controller.ObtenerPorId(99, CancellationToken.None);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFound.StatusCode);
    }

    [Fact]
    public async Task Listar_RetornaOk_ConListaDeClientes()
    {
        var clientes = new List<ClienteResponse>
        {
            new() { Id = 1, Nombre = "Cliente A", Activo = true },
            new() { Id = 2, Nombre = "Cliente B", Activo = true }
        };

        _clienteServiceMock
            .Setup(s => s.ListarAsync(true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(clientes);

        var result = await _controller.Listar(true, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(clientes, ok.Value);
    }

    [Fact]
    public async Task Actualizar_RetornaOk_CuandoActualizacionEsExitosa()
    {
        var request = new ActualizarClienteRequest { Nombre = "Nombre actualizado" };
        var cliente = new ClienteResponse { Id = 1, Nombre = request.Nombre! };

        _clienteServiceMock
            .Setup(s => s.ActualizarAsync(1, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((cliente, null));

        var result = await _controller.Actualizar(1, request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(cliente, ok.Value);
    }

    [Fact]
    public async Task Actualizar_RetornaNotFound_CuandoClienteNoExiste()
    {
        var request = new ActualizarClienteRequest { Nombre = "Nombre" };

        _clienteServiceMock
            .Setup(s => s.ActualizarAsync(99, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((null, "Cliente no encontrado."));

        var result = await _controller.Actualizar(99, request, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Actualizar_RetornaBadRequest_CuandoHayErrorDeValidacion()
    {
        var request = new ActualizarClienteRequest { Nombre = "" };

        _clienteServiceMock
            .Setup(s => s.ActualizarAsync(1, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((null, "El nombre es obligatorio."));

        var result = await _controller.Actualizar(1, request, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Eliminar_RetornaNoContent_CuandoEliminacionEsExitosa()
    {
        _clienteServiceMock
            .Setup(s => s.EliminarAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _controller.Eliminar(1, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Eliminar_RetornaNotFound_CuandoClienteNoExiste()
    {
        _clienteServiceMock
            .Setup(s => s.EliminarAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _controller.Eliminar(99, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
