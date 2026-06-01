using Microsoft.AspNetCore.Mvc;
using Moq;
using SistemaMantenimientoBackend.Controllers;
using SistemaMantenimientoBackend.Models.Requests;
using SistemaMantenimientoBackend.Models.Responses;
using SistemaMantenimientoBackend.Services;

namespace SistemaMantenimientoBackend.Tests.Controllers;

public class TecnicosControllerTests
{
    private readonly Mock<ITecnicoService> _tecnicoServiceMock = new();
    private readonly TecnicosController _controller;

    public TecnicosControllerTests()
    {
        _controller = new TecnicosController(_tecnicoServiceMock.Object);
    }

    [Fact]
    public async Task Crear_RetornaCreated_CuandoServicioEsExitoso()
    {
        var request = new CrearTecnicoRequest
        {
            Nombre = "Carlos Gómez",
            Telefono = "3009876543",
            Especialidad = "Electricidad"
        };
        var tecnico = new TecnicoResponse
        {
            Id = 1,
            Nombre = request.Nombre,
            Telefono = request.Telefono,
            Especialidad = request.Especialidad,
            Activo = true
        };

        _tecnicoServiceMock
            .Setup(s => s.CrearAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((tecnico, null));

        var result = await _controller.Crear(request, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, created.StatusCode);
        Assert.Equal(tecnico, created.Value);
    }

    [Fact]
    public async Task Crear_RetornaBadRequest_CuandoServicioRetornaError()
    {
        var request = new CrearTecnicoRequest { Nombre = "Duplicado" };

        _tecnicoServiceMock
            .Setup(s => s.CrearAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((null, "El técnico ya existe."));

        var result = await _controller.Crear(request, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ObtenerPorId_RetornaOk_CuandoTecnicoExiste()
    {
        var tecnico = new TecnicoResponse { Id = 1, Nombre = "Carlos Gómez" };

        _tecnicoServiceMock
            .Setup(s => s.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tecnico);

        var result = await _controller.ObtenerPorId(1, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(tecnico, ok.Value);
    }

    [Fact]
    public async Task ObtenerPorId_RetornaNotFound_CuandoTecnicoNoExiste()
    {
        _tecnicoServiceMock
            .Setup(s => s.ObtenerPorIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TecnicoResponse?)null);

        var result = await _controller.ObtenerPorId(99, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Listar_RetornaOk_ConListaDeTecnicos()
    {
        var tecnicos = new List<TecnicoResponse>
        {
            new() { Id = 1, Nombre = "Técnico A", Activo = true }
        };

        _tecnicoServiceMock
            .Setup(s => s.ListarAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tecnicos);

        var result = await _controller.Listar(null, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(tecnicos, ok.Value);
    }

    [Fact]
    public async Task Actualizar_RetornaOk_CuandoActualizacionEsExitosa()
    {
        var request = new ActualizarTecnicoRequest { Nombre = "Nombre actualizado" };
        var tecnico = new TecnicoResponse { Id = 1, Nombre = request.Nombre! };

        _tecnicoServiceMock
            .Setup(s => s.ActualizarAsync(1, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((tecnico, null));

        var result = await _controller.Actualizar(1, request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(tecnico, ok.Value);
    }

    [Fact]
    public async Task Actualizar_RetornaNotFound_CuandoTecnicoNoExiste()
    {
        var request = new ActualizarTecnicoRequest { Nombre = "Nombre" };

        _tecnicoServiceMock
            .Setup(s => s.ActualizarAsync(99, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((null, "Técnico no encontrado."));

        var result = await _controller.Actualizar(99, request, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Actualizar_RetornaBadRequest_CuandoHayErrorDeValidacion()
    {
        var request = new ActualizarTecnicoRequest { Nombre = "" };

        _tecnicoServiceMock
            .Setup(s => s.ActualizarAsync(1, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((null, "El nombre es obligatorio."));

        var result = await _controller.Actualizar(1, request, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Eliminar_RetornaNoContent_CuandoEliminacionEsExitosa()
    {
        _tecnicoServiceMock
            .Setup(s => s.EliminarAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _controller.Eliminar(1, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Eliminar_RetornaNotFound_CuandoTecnicoNoExiste()
    {
        _tecnicoServiceMock
            .Setup(s => s.EliminarAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _controller.Eliminar(99, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
