using Moq;
using SistemaMantenimientoBackend.Models;
using SistemaMantenimientoBackend.Models.Enums;
using SistemaMantenimientoBackend.Models.Requests;
using SistemaMantenimientoBackend.Repositories;
using SistemaMantenimientoBackend.Services;

namespace SistemaMantenimientoBackend.Tests.Services;

public class OrdenServicioServiceTests
{
    private readonly Mock<IOrdenServicioRepository> _ordenRepositoryMock = new();
    private readonly Mock<IClienteRepository> _clienteRepositoryMock = new();
    private readonly Mock<ITecnicoRepository> _tecnicoRepositoryMock = new();
    private readonly OrdenServicioService _service;

    public OrdenServicioServiceTests()
    {
        _service = new OrdenServicioService(
            _ordenRepositoryMock.Object,
            _clienteRepositoryMock.Object,
            _tecnicoRepositoryMock.Object);
    }

    [Fact]
    public async Task CrearAsync_RetornaError_CuandoClienteNoExiste()
    {
        var request = new CrearOrdenServicioRequest
        {
            Descripcion = "Reparación",
            ClienteId = 1,
            TecnicoId = 2,
            Estado = EstadoOrdenServicio.Pendiente
        };

        _clienteRepositoryMock
            .Setup(r => r.ExisteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var (orden, error) = await _service.CrearAsync(request, CancellationToken.None);

        Assert.Null(orden);
        Assert.Equal("El cliente especificado no existe o no está activo.", error);
        _ordenRepositoryMock.Verify(r => r.CrearAsync(It.IsAny<OrdenServicio>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CrearAsync_RetornaError_CuandoTecnicoNoExiste()
    {
        var request = new CrearOrdenServicioRequest
        {
            Descripcion = "Reparación",
            ClienteId = 1,
            TecnicoId = 2,
            Estado = EstadoOrdenServicio.Pendiente
        };

        _clienteRepositoryMock
            .Setup(r => r.ExisteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _tecnicoRepositoryMock
            .Setup(r => r.ExisteAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var (orden, error) = await _service.CrearAsync(request, CancellationToken.None);

        Assert.Null(orden);
        Assert.Equal("El técnico especificado no existe o no está activo.", error);
    }

    [Fact]
    public async Task CrearAsync_RetornaOrden_CuandoDatosSonValidos()
    {
        var request = new CrearOrdenServicioRequest
        {
            Descripcion = "  Reparación de equipo  ",
            ClienteId = 1,
            TecnicoId = 2,
            Estado = EstadoOrdenServicio.Pendiente
        };
        var creada = CrearOrdenEjemplo(10, "Reparación de equipo", "pendiente");

        ConfigurarReferenciasValidas(1, 2);

        _ordenRepositoryMock
            .Setup(r => r.CrearAsync(It.Is<OrdenServicio>(o =>
                o.Descripcion == "Reparación de equipo" &&
                o.ClienteId == 1 &&
                o.TecnicoId == 2 &&
                o.Estado == "pendiente"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);

        _ordenRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(creada);

        var (orden, error) = await _service.CrearAsync(request, CancellationToken.None);

        Assert.Null(error);
        Assert.NotNull(orden);
        Assert.Equal(10, orden.Id);
        Assert.Equal("Reparación de equipo", orden.Descripcion);
        Assert.Equal(EstadoOrdenServicio.Pendiente, orden.Estado);
        Assert.Equal("Pendiente", orden.EstadoDescripcion);
        Assert.Equal("Carlos López", orden.TecnicoNombre);
        Assert.Equal("Juan Pérez", orden.ClienteNombre);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_RetornaNull_CuandoOrdenNoExiste()
    {
        _ordenRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrdenServicio?)null);

        var result = await _service.ObtenerPorIdAsync(99, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task ListarAsync_RetornaListaMapeada()
    {
        var filtros = new ListarOrdenesServicioFiltros { ClienteId = 1 };
        var ordenes = new List<OrdenServicio>
        {
            CrearOrdenEjemplo(1, "Orden A", "pendiente"),
            CrearOrdenEjemplo(2, "Orden B", "en_progreso")
        };

        _ordenRepositoryMock
            .Setup(r => r.ListarAsync(filtros, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordenes);

        var result = await _service.ListarAsync(filtros, CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Equal(EstadoOrdenServicio.Pendiente, result[0].Estado);
        Assert.Equal(EstadoOrdenServicio.EnProgreso, result[1].Estado);
    }

    [Fact]
    public async Task ActualizarAsync_RetornaError_CuandoOrdenNoExiste()
    {
        var request = new ActualizarOrdenServicioRequest
        {
            Descripcion = "Actualizada",
            ClienteId = 1,
            TecnicoId = 2,
            Estado = EstadoOrdenServicio.EnProgreso
        };

        _ordenRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrdenServicio?)null);

        var (orden, error) = await _service.ActualizarAsync(99, request, CancellationToken.None);

        Assert.Null(orden);
        Assert.Equal("Orden de servicio no encontrada.", error);
    }

    [Fact]
    public async Task ActualizarAsync_RetornaOrdenActualizada_CuandoDatosSonValidos()
    {
        var existente = CrearOrdenEjemplo(5, "Descripción anterior", "pendiente");
        var request = new ActualizarOrdenServicioRequest
        {
            Descripcion = "  Descripción actualizada  ",
            ClienteId = 1,
            TecnicoId = 2,
            Estado = EstadoOrdenServicio.Finalizada
        };
        var actualizada = CrearOrdenEjemplo(5, "Descripción actualizada", "finalizada");

        ConfigurarReferenciasValidas(1, 2);

        _ordenRepositoryMock
            .SetupSequence(r => r.ObtenerPorIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existente)
            .ReturnsAsync(actualizada);

        _ordenRepositoryMock
            .Setup(r => r.ActualizarAsync(It.Is<OrdenServicio>(o =>
                o.Descripcion == "Descripción actualizada" &&
                o.Estado == "finalizada"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var (orden, error) = await _service.ActualizarAsync(5, request, CancellationToken.None);

        Assert.Null(error);
        Assert.NotNull(orden);
        Assert.Equal("Descripción actualizada", orden.Descripcion);
        Assert.Equal(EstadoOrdenServicio.Finalizada, orden.Estado);
    }

    [Fact]
    public async Task CambiarEstadoAsync_RetornaError_CuandoOrdenNoExiste()
    {
        var request = new CambiarEstadoOrdenServicioRequest
        {
            Estado = EstadoOrdenServicio.EnProgreso
        };

        _ordenRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrdenServicio?)null);

        var (orden, error) = await _service.CambiarEstadoAsync(99, request, CancellationToken.None);

        Assert.Null(orden);
        Assert.Equal("Orden de servicio no encontrada.", error);
    }

    [Fact]
    public async Task CambiarEstadoAsync_RetornaOrdenActualizada_CuandoExiste()
    {
        var existente = CrearOrdenEjemplo(7, "Orden", "pendiente");
        var actualizada = CrearOrdenEjemplo(7, "Orden", "en_progreso");
        var request = new CambiarEstadoOrdenServicioRequest
        {
            Estado = EstadoOrdenServicio.EnProgreso
        };

        _ordenRepositoryMock
            .SetupSequence(r => r.ObtenerPorIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existente)
            .ReturnsAsync(actualizada);

        _ordenRepositoryMock
            .Setup(r => r.ActualizarEstadoAsync(7, "en_progreso", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var (orden, error) = await _service.CambiarEstadoAsync(7, request, CancellationToken.None);

        Assert.Null(error);
        Assert.NotNull(orden);
        Assert.Equal(EstadoOrdenServicio.EnProgreso, orden.Estado);
        Assert.Equal("En progreso", orden.EstadoDescripcion);
    }

    [Fact]
    public async Task EliminarAsync_RetornaResultadoDelRepositorio()
    {
        _ordenRepositoryMock
            .Setup(r => r.EliminarAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.EliminarAsync(3, CancellationToken.None);

        Assert.True(result);
    }

    private void ConfigurarReferenciasValidas(int clienteId, int tecnicoId)
    {
        _clienteRepositoryMock
            .Setup(r => r.ExisteAsync(clienteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _tecnicoRepositoryMock
            .Setup(r => r.ExisteAsync(tecnicoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    private static OrdenServicio CrearOrdenEjemplo(int id, string descripcion, string estado) => new()
    {
        Id = id,
        FechaCreacion = new DateTime(2026, 5, 31, 10, 0, 0, DateTimeKind.Utc),
        Descripcion = descripcion,
        Estado = estado,
        ClienteId = 1,
        TecnicoId = 2,
        ClienteNombre = "Juan Pérez",
        ClienteDocumentoIdentidad = "12345678",
        TecnicoNombre = "Carlos López",
        TecnicoEspecialidad = "Electricidad"
    };
}
