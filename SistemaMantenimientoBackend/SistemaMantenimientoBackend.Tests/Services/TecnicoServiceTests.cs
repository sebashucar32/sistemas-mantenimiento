using Moq;
using SistemaMantenimientoBackend.Models;
using SistemaMantenimientoBackend.Models.Requests;
using SistemaMantenimientoBackend.Repositories;
using SistemaMantenimientoBackend.Services;

namespace SistemaMantenimientoBackend.Tests.Services;

public class TecnicoServiceTests
{
    private readonly Mock<ITecnicoRepository> _tecnicoRepositoryMock = new();
    private readonly TecnicoService _service;

    public TecnicoServiceTests()
    {
        _service = new TecnicoService(_tecnicoRepositoryMock.Object);
    }

    [Fact]
    public async Task CrearAsync_RetornaTecnico_CuandoDatosSonValidos()
    {
        var request = new CrearTecnicoRequest
        {
            Nombre = "  Carlos López  ",
            Telefono = " 3009876543 ",
            Especialidad = " Electricidad "
        };
        var creado = new Tecnico
        {
            Id = 1,
            Nombre = "Carlos López",
            Telefono = "3009876543",
            Especialidad = "Electricidad",
            Activo = true
        };

        _tecnicoRepositoryMock
            .Setup(r => r.CrearAsync(It.Is<Tecnico>(t =>
                t.Nombre == "Carlos López" &&
                t.Telefono == "3009876543" &&
                t.Especialidad == "Electricidad" &&
                t.Activo), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _tecnicoRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(creado);

        var (tecnico, error) = await _service.CrearAsync(request, CancellationToken.None);

        Assert.Null(error);
        Assert.NotNull(tecnico);
        Assert.Equal(1, tecnico.Id);
        Assert.Equal("Carlos López", tecnico.Nombre);
        Assert.Equal("Electricidad", tecnico.Especialidad);
        Assert.True(tecnico.Activo);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_RetornaNull_CuandoTecnicoNoExiste()
    {
        _tecnicoRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tecnico?)null);

        var result = await _service.ObtenerPorIdAsync(99, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_RetornaTecnico_CuandoExiste()
    {
        var tecnico = new Tecnico { Id = 2, Nombre = "Ana Ruiz", Activo = true };

        _tecnicoRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tecnico);

        var result = await _service.ObtenerPorIdAsync(2, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Id);
        Assert.Equal("Ana Ruiz", result.Nombre);
    }

    [Fact]
    public async Task ListarAsync_RetornaListaMapeada()
    {
        var tecnicos = new List<Tecnico>
        {
            new() { Id = 1, Nombre = "Técnico A", Activo = true },
            new() { Id = 2, Nombre = "Técnico B", Activo = false }
        };

        _tecnicoRepositoryMock
            .Setup(r => r.ListarAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tecnicos);

        var result = await _service.ListarAsync(cancellationToken: CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Equal("Técnico A", result[0].Nombre);
    }

    [Fact]
    public async Task ActualizarAsync_RetornaError_CuandoTecnicoNoExiste()
    {
        var request = new ActualizarTecnicoRequest { Nombre = "Nombre" };

        _tecnicoRepositoryMock
            .Setup(r => r.ObtenerPorIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tecnico?)null);

        var (tecnico, error) = await _service.ActualizarAsync(99, request, CancellationToken.None);

        Assert.Null(tecnico);
        Assert.Equal("Técnico no encontrado.", error);
    }

    [Fact]
    public async Task ActualizarAsync_RetornaTecnicoActualizado_CuandoDatosSonValidos()
    {
        var existente = new Tecnico
        {
            Id = 1,
            Nombre = "Anterior",
            Telefono = "3001111111",
            Especialidad = "Plomería",
            Activo = true
        };
        var request = new ActualizarTecnicoRequest
        {
            Nombre = "  Técnico Actualizado  ",
            Telefono = " 3002222222 ",
            Especialidad = " Electricidad ",
            Activo = false
        };
        var actualizado = new Tecnico
        {
            Id = 1,
            Nombre = "Técnico Actualizado",
            Telefono = "3002222222",
            Especialidad = "Electricidad",
            Activo = false
        };

        _tecnicoRepositoryMock
            .SetupSequence(r => r.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existente)
            .ReturnsAsync(actualizado);

        _tecnicoRepositoryMock
            .Setup(r => r.ActualizarAsync(It.Is<Tecnico>(t =>
                t.Nombre == "Técnico Actualizado" &&
                t.Telefono == "3002222222" &&
                t.Especialidad == "Electricidad" &&
                t.Activo == false), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var (tecnico, error) = await _service.ActualizarAsync(1, request, CancellationToken.None);

        Assert.Null(error);
        Assert.NotNull(tecnico);
        Assert.Equal("Técnico Actualizado", tecnico.Nombre);
        Assert.False(tecnico.Activo);
    }

    [Fact]
    public async Task EliminarAsync_RetornaResultadoDelRepositorio()
    {
        _tecnicoRepositoryMock
            .Setup(r => r.EliminarAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.EliminarAsync(1, CancellationToken.None);

        Assert.False(result);
    }
}
