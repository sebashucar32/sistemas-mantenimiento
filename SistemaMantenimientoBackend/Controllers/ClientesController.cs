using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaMantenimientoBackend.Models.Requests;
using SistemaMantenimientoBackend.Models.Responses;
using SistemaMantenimientoBackend.Services;

namespace SistemaMantenimientoBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController(IClienteService clienteService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Crear(
        [FromBody] CrearClienteRequest request,
        CancellationToken cancellationToken)
    {
        var (cliente, error) = await clienteService.CrearAsync(request, cancellationToken);

        if (error is not null)
        {
            return BadRequest(new { mensaje = error });
        }

        return CreatedAtAction(nameof(ObtenerPorId), new { id = cliente!.Id }, cliente);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorId(int id, CancellationToken cancellationToken)
    {
        var cliente = await clienteService.ObtenerPorIdAsync(id, cancellationToken);

        if (cliente is null)
        {
            return NotFound(new { mensaje = "Cliente no encontrado." });
        }

        return Ok(cliente);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ClienteResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
        [FromQuery] bool? activo,
        CancellationToken cancellationToken)
    {
        var clientes = await clienteService.ListarAsync(activo, cancellationToken);
        return Ok(clientes);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] ActualizarClienteRequest request,
        CancellationToken cancellationToken)
    {
        var (cliente, error) = await clienteService.ActualizarAsync(id, request, cancellationToken);

        if (error is not null)
        {
            if (error.Contains("no encontrado"))
            {
                return NotFound(new { mensaje = error });
            }

            return BadRequest(new { mensaje = error });
        }

        return Ok(cliente);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken)
    {
        var eliminado = await clienteService.EliminarAsync(id, cancellationToken);

        if (!eliminado)
        {
            return NotFound(new { mensaje = "Cliente no encontrado." });
        }

        return NoContent();
    }
}
