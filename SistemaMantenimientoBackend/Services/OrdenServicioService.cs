using SistemaMantenimientoBackend.Models;
using SistemaMantenimientoBackend.Models.Enums;
using SistemaMantenimientoBackend.Models.Requests;
using SistemaMantenimientoBackend.Models.Responses;
using SistemaMantenimientoBackend.Repositories;

namespace SistemaMantenimientoBackend.Services;

public class OrdenServicioService(
    IOrdenServicioRepository ordenServicioRepository,
    IClienteRepository clienteRepository,
    ITecnicoRepository tecnicoRepository) : IOrdenServicioService
{
    public async Task<(OrdenServicioResponse? Orden, string? Error)> CrearAsync(
        CrearOrdenServicioRequest request,
        CancellationToken cancellationToken = default)
    {
        var errorValidacion = await ValidarReferenciasAsync(
            request.ClienteId,
            request.TecnicoId,
            cancellationToken);

        if (errorValidacion is not null)
        {
            return (null, errorValidacion);
        }

        var orden = new OrdenServicio
        {
            Descripcion = request.Descripcion.Trim(),
            TecnicoId = request.TecnicoId,
            ClienteId = request.ClienteId,
            Estado = EstadoOrdenServicioMapper.AValorBaseDatos(request.Estado)
        };

        var id = await ordenServicioRepository.CrearAsync(orden, cancellationToken);
        var creada = await ordenServicioRepository.ObtenerPorIdAsync(id, cancellationToken);

        return (creada is null ? null : MapearOrden(creada), null);
    }

    public async Task<OrdenServicioResponse?> ObtenerPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var orden = await ordenServicioRepository.ObtenerPorIdAsync(id, cancellationToken);
        return orden is null ? null : MapearOrden(orden);
    }

    public async Task<IReadOnlyList<OrdenServicioResponse>> ListarAsync(
        ListarOrdenesServicioFiltros filtros,
        CancellationToken cancellationToken = default)
    {
        var ordenes = await ordenServicioRepository.ListarAsync(filtros, cancellationToken);
        return ordenes.Select(MapearOrden).ToList();
    }

    public async Task<(OrdenServicioResponse? Orden, string? Error)> ActualizarAsync(
        int id,
        ActualizarOrdenServicioRequest request,
        CancellationToken cancellationToken = default)
    {
        var existente = await ordenServicioRepository.ObtenerPorIdAsync(id, cancellationToken);

        if (existente is null)
        {
            return (null, "Orden de servicio no encontrada.");
        }

        var errorValidacion = await ValidarReferenciasAsync(
            request.ClienteId,
            request.TecnicoId,
            cancellationToken);

        if (errorValidacion is not null)
        {
            return (null, errorValidacion);
        }

        existente.Descripcion = request.Descripcion.Trim();
        existente.TecnicoId = request.TecnicoId;
        existente.ClienteId = request.ClienteId;
        existente.Estado = EstadoOrdenServicioMapper.AValorBaseDatos(request.Estado);

        await ordenServicioRepository.ActualizarAsync(existente, cancellationToken);

        var actualizada = await ordenServicioRepository.ObtenerPorIdAsync(id, cancellationToken);
        return (actualizada is null ? null : MapearOrden(actualizada), null);
    }

    public async Task<(OrdenServicioResponse? Orden, string? Error)> CambiarEstadoAsync(
        int id,
        CambiarEstadoOrdenServicioRequest request,
        CancellationToken cancellationToken = default)
    {
        var existente = await ordenServicioRepository.ObtenerPorIdAsync(id, cancellationToken);

        if (existente is null)
        {
            return (null, "Orden de servicio no encontrada.");
        }

        var estadoDb = EstadoOrdenServicioMapper.AValorBaseDatos(request.Estado);
        await ordenServicioRepository.ActualizarEstadoAsync(id, estadoDb, cancellationToken);

        var actualizada = await ordenServicioRepository.ObtenerPorIdAsync(id, cancellationToken);
        return (actualizada is null ? null : MapearOrden(actualizada), null);
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        return await ordenServicioRepository.EliminarAsync(id, cancellationToken);
    }

    private async Task<string?> ValidarReferenciasAsync(
        int clienteId,
        int tecnicoId,
        CancellationToken cancellationToken)
    {
        if (!await clienteRepository.ExisteAsync(clienteId, cancellationToken))
        {
            return "El cliente especificado no existe o no está activo.";
        }

        if (!await tecnicoRepository.ExisteAsync(tecnicoId, cancellationToken))
        {
            return "El técnico especificado no existe o no está activo.";
        }

        return null;
    }

    private static OrdenServicioResponse MapearOrden(OrdenServicio orden)
    {
        var estado = EstadoOrdenServicioMapper.DesdeValorBaseDatos(orden.Estado);

        return new OrdenServicioResponse
        {
            Id = orden.Id,
            FechaCreacion = orden.FechaCreacion,
            Estado = estado,
            EstadoDescripcion = EstadoOrdenServicioMapper.AEtiqueta(estado),
            Descripcion = orden.Descripcion,
            TecnicoId = orden.TecnicoId,
            TecnicoNombre = orden.TecnicoNombre ?? string.Empty,
            TecnicoEspecialidad = orden.TecnicoEspecialidad ?? string.Empty,
            ClienteId = orden.ClienteId,
            ClienteNombre = orden.ClienteNombre ?? string.Empty,
            ClienteDocumentoIdentidad = orden.ClienteDocumentoIdentidad ?? string.Empty
        };
    }
}
