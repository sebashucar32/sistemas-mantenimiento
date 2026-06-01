using Microsoft.AspNetCore.Mvc;
using Moq;
using SistemaMantenimientoBackend.Controllers;
using SistemaMantenimientoBackend.Models.Enums;
using SistemaMantenimientoBackend.Models.Requests;
using SistemaMantenimientoBackend.Models.Responses;
using SistemaMantenimientoBackend.Services;

namespace SistemaMantenimientoBackend.Tests.Controllers;

public class OrdenesServicioControllerTests
{
    private readonly Mock<IOrdenServicioService> _ordenServicioServiceMock = new();
    private readonly OrdenesServicioController _controller;

    public OrdenesServicioControllerTests()
    {
        _controller = new OrdenesServicioController(_ordenServicioServiceMock.Object);
    }

    [Fact]
    public async Task Crear_RetornaCreated_CuandoServicioEsExitoso()
    {
        var request = new CrearOrdenServicioRequest
        {
            Descripcion = "Reparación eléctrica",
            TecnicoId = 1,
            ClienteId = 2
        };
        var orden = new OrdenServicioResponse
        {
            Id = 1,
            Descripcion = request.Descripcion,
            TecnicoId = request.TecnicoId,
            ClienteId = request.ClienteId,
            Estado = EstadoOrdenServicio.Pendiente
        };

        _ordenServicioServiceMock
            .Setup(s => s.CrearAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((orden, null));

        var result = await _controller.Crear(request, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, created.StatusCode);
        Assert.Equal(orden, created.Value);
    }

    [Fact]
    public async Task Crear_RetornaBadRequest_CuandoServicioRetornaError()
    {
        var request = new CrearOrdenServicioRequest { Descripcion = "Orden inválida" };

        _ordenServicioServiceMock
            .Setup(s => s.CrearAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((null, "El cliente no existe."));

        var result = await _controller.Crear(request, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ObtenerPorId_RetornaOk_CuandoOrdenExiste()
    {
        var orden = new OrdenServicioResponse { Id = 1, Descripcion = "Reparación" };

        _ordenServicioServiceMock
            .Setup(s => s.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orden);

        var result = await _controller.ObtenerPorId(1, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(orden, ok.Value);
    }

    [Fact]
    public async Task ObtenerPorId_RetornaNotFound_CuandoOrdenNoExiste()
    {
        _ordenServicioServiceMock
            .Setup(s => s.ObtenerPorIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrdenServicioResponse?)null);

        var result = await _controller.ObtenerPorId(99, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Listar_RetornaOk_ConListaDeOrdenes()
    {
        var filtros = new ListarOrdenesServicioFiltros { Estado = EstadoOrdenServicio.Pendiente };
        var ordenes = new List<OrdenServicioResponse>
        {
            new() { Id = 1, Estado = EstadoOrdenServicio.Pendiente }
        };

        _ordenServicioServiceMock
            .Setup(s => s.ListarAsync(filtros, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordenes);

        var result = await _controller.Listar(filtros, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(ordenes, ok.Value);
    }

    [Fact]
    public async Task Actualizar_RetornaOk_CuandoActualizacionEsExitosa()
    {
        var request = new ActualizarOrdenServicioRequest { Descripcion = "Descripción actualizada" };
        var orden = new OrdenServicioResponse { Id = 1, Descripcion = request.Descripcion! };

        _ordenServicioServiceMock
            .Setup(s => s.ActualizarAsync(1, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((orden, null));

        var result = await _controller.Actualizar(1, request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(orden, ok.Value);
    }

    [Fact]
    public async Task Actualizar_RetornaNotFound_CuandoOrdenNoExiste()
    {
        var request = new ActualizarOrdenServicioRequest { Descripcion = "Descripción" };

        _ordenServicioServiceMock
            .Setup(s => s.ActualizarAsync(99, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((null, "Orden de servicio no encontrada."));

        var result = await _controller.Actualizar(99, request, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Actualizar_RetornaBadRequest_CuandoHayErrorDeValidacion()
    {
        var request = new ActualizarOrdenServicioRequest { Descripcion = "" };

        _ordenServicioServiceMock
            .Setup(s => s.ActualizarAsync(1, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((null, "La descripción es obligatoria."));

        var result = await _controller.Actualizar(1, request, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CambiarEstado_RetornaOk_CuandoCambioEsExitoso()
    {
        var request = new CambiarEstadoOrdenServicioRequest { Estado = EstadoOrdenServicio.EnProgreso };
        var orden = new OrdenServicioResponse { Id = 1, Estado = EstadoOrdenServicio.EnProgreso };

        _ordenServicioServiceMock
            .Setup(s => s.CambiarEstadoAsync(1, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((orden, null));

        var result = await _controller.CambiarEstado(1, request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(orden, ok.Value);
    }

    [Fact]
    public async Task CambiarEstado_RetornaNotFound_CuandoOrdenNoExiste()
    {
        var request = new CambiarEstadoOrdenServicioRequest { Estado = EstadoOrdenServicio.Finalizada };

        _ordenServicioServiceMock
            .Setup(s => s.CambiarEstadoAsync(99, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((null, "Orden de servicio no encontrada."));

        var result = await _controller.CambiarEstado(99, request, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Eliminar_RetornaNoContent_CuandoEliminacionEsExitosa()
    {
        _ordenServicioServiceMock
            .Setup(s => s.EliminarAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _controller.Eliminar(1, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Eliminar_RetornaNotFound_CuandoOrdenNoExiste()
    {
        _ordenServicioServiceMock
            .Setup(s => s.EliminarAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _controller.Eliminar(99, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
