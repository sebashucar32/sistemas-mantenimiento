using SistemaMantenimientoBackend.Models;
using SistemaMantenimientoBackend.Models.Requests;
using SistemaMantenimientoBackend.Models.Responses;
using SistemaMantenimientoBackend.Repositories;

namespace SistemaMantenimientoBackend.Services;

public class ClienteService(IClienteRepository clienteRepository) : IClienteService
{
    public async Task<(ClienteResponse? Cliente, string? Error)> CrearAsync(
        CrearClienteRequest request,
        CancellationToken cancellationToken = default)
    {
        var documento = request.DocumentoIdentidad.Trim();

        if (await clienteRepository.ExisteDocumentoAsync(documento, cancellationToken: cancellationToken))
        {
            return (null, "Ya existe un cliente con el mismo documento de identidad.");
        }

        var cliente = new Cliente
        {
            Nombre = request.Nombre.Trim(),
            DocumentoIdentidad = documento,
            Direccion = request.Direccion.Trim(),
            Telefono = request.Telefono.Trim(),
            Activo = true
        };

        var id = await clienteRepository.CrearAsync(cliente, cancellationToken);
        var creado = await clienteRepository.ObtenerPorIdAsync(id, cancellationToken);

        return (creado is null ? null : MapearCliente(creado), null);
    }

    public async Task<ClienteResponse?> ObtenerPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var cliente = await clienteRepository.ObtenerPorIdAsync(id, cancellationToken);
        return cliente is null ? null : MapearCliente(cliente);
    }

    public async Task<IReadOnlyList<ClienteResponse>> ListarAsync(
        bool? activo = null,
        CancellationToken cancellationToken = default)
    {
        var clientes = await clienteRepository.ListarAsync(activo, cancellationToken);
        return clientes.Select(MapearCliente).ToList();
    }

    public async Task<(ClienteResponse? Cliente, string? Error)> ActualizarAsync(
        int id,
        ActualizarClienteRequest request,
        CancellationToken cancellationToken = default)
    {
        var existente = await clienteRepository.ObtenerPorIdAsync(id, cancellationToken);

        if (existente is null)
        {
            return (null, "Cliente no encontrado.");
        }

        var documento = request.DocumentoIdentidad.Trim();

        if (await clienteRepository.ExisteDocumentoAsync(documento, id, cancellationToken))
        {
            return (null, "Ya existe un cliente con el mismo documento de identidad.");
        }

        existente.Nombre = request.Nombre.Trim();
        existente.DocumentoIdentidad = documento;
        existente.Direccion = request.Direccion.Trim();
        existente.Telefono = request.Telefono.Trim();
        existente.Activo = request.Activo;

        await clienteRepository.ActualizarAsync(existente, cancellationToken);

        var actualizado = await clienteRepository.ObtenerPorIdAsync(id, cancellationToken);
        return (actualizado is null ? null : MapearCliente(actualizado), null);
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        return await clienteRepository.EliminarAsync(id, cancellationToken);
    }

    private static ClienteResponse MapearCliente(Cliente cliente)
    {
        return new ClienteResponse
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            DocumentoIdentidad = cliente.DocumentoIdentidad,
            Direccion = cliente.Direccion,
            Telefono = cliente.Telefono,
            Activo = cliente.Activo
        };
    }
}
