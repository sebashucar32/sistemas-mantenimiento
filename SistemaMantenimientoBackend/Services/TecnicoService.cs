using SistemaMantenimientoBackend.Models;
using SistemaMantenimientoBackend.Models.Requests;
using SistemaMantenimientoBackend.Models.Responses;
using SistemaMantenimientoBackend.Repositories;

namespace SistemaMantenimientoBackend.Services;

public class TecnicoService(ITecnicoRepository tecnicoRepository) : ITecnicoService
{
    public async Task<(TecnicoResponse? Tecnico, string? Error)> CrearAsync(
        CrearTecnicoRequest request,
        CancellationToken cancellationToken = default)
    {
        var tecnico = new Tecnico
        {
            Nombre = request.Nombre.Trim(),
            Telefono = request.Telefono.Trim(),
            Especialidad = request.Especialidad.Trim(),
            Activo = true
        };

        var id = await tecnicoRepository.CrearAsync(tecnico, cancellationToken);
        var creado = await tecnicoRepository.ObtenerPorIdAsync(id, cancellationToken);

        return (creado is null ? null : MapearTecnico(creado), null);
    }

    public async Task<TecnicoResponse?> ObtenerPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var tecnico = await tecnicoRepository.ObtenerPorIdAsync(id, cancellationToken);
        return tecnico is null ? null : MapearTecnico(tecnico);
    }

    public async Task<IReadOnlyList<TecnicoResponse>> ListarAsync(
        bool? activo = null,
        CancellationToken cancellationToken = default)
    {
        var tecnicos = await tecnicoRepository.ListarAsync(activo, cancellationToken);
        return tecnicos.Select(MapearTecnico).ToList();
    }

    public async Task<(TecnicoResponse? Tecnico, string? Error)> ActualizarAsync(
        int id,
        ActualizarTecnicoRequest request,
        CancellationToken cancellationToken = default)
    {
        var existente = await tecnicoRepository.ObtenerPorIdAsync(id, cancellationToken);

        if (existente is null)
        {
            return (null, "Técnico no encontrado.");
        }

        existente.Nombre = request.Nombre.Trim();
        existente.Telefono = request.Telefono.Trim();
        existente.Especialidad = request.Especialidad.Trim();
        existente.Activo = request.Activo;

        await tecnicoRepository.ActualizarAsync(existente, cancellationToken);

        var actualizado = await tecnicoRepository.ObtenerPorIdAsync(id, cancellationToken);
        return (actualizado is null ? null : MapearTecnico(actualizado), null);
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        return await tecnicoRepository.EliminarAsync(id, cancellationToken);
    }

    private static TecnicoResponse MapearTecnico(Tecnico tecnico)
    {
        return new TecnicoResponse
        {
            Id = tecnico.Id,
            Nombre = tecnico.Nombre,
            Telefono = tecnico.Telefono,
            Especialidad = tecnico.Especialidad,
            Activo = tecnico.Activo
        };
    }
}
