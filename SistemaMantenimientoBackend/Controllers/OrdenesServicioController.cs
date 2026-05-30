using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaMantenimientoBackend.Models.Requests;
using SistemaMantenimientoBackend.Models.Responses;
using SistemaMantenimientoBackend.Services;

namespace SistemaMantenimientoBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdenesServicioController(IOrdenServicioService ordenServicioService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(OrdenServicioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Crear(
        [FromBody] CrearOrdenServicioRequest request,
        CancellationToken cancellationToken)
    {
        var (orden, error) = await ordenServicioService.CrearAsync(request, cancellationToken);

        if (error is not null)
        {
            return BadRequest(new { mensaje = error });
        }

        return CreatedAtAction(nameof(ObtenerPorId), new { id = orden!.Id }, orden);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrdenServicioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorId(int id, CancellationToken cancellationToken)
    {
        var orden = await ordenServicioService.ObtenerPorIdAsync(id, cancellationToken);

        if (orden is null)
        {
            return NotFound(new { mensaje = "Orden de servicio no encontrada." });
        }

        return Ok(orden);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OrdenServicioResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
        [FromQuery] ListarOrdenesServicioFiltros filtros,
        CancellationToken cancellationToken)
    {
        var ordenes = await ordenServicioService.ListarAsync(filtros, cancellationToken);
        return Ok(ordenes);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(OrdenServicioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] ActualizarOrdenServicioRequest request,
        CancellationToken cancellationToken)
    {
        var (orden, error) = await ordenServicioService.ActualizarAsync(id, request, cancellationToken);

        if (error is not null)
        {
            if (error.Contains("no encontrada"))
            {
                return NotFound(new { mensaje = error });
            }

            return BadRequest(new { mensaje = error });
        }

        return Ok(orden);
    }

    [HttpPatch("{id:int}/estado")]
    [ProducesResponseType(typeof(OrdenServicioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CambiarEstado(
        int id,
        [FromBody] CambiarEstadoOrdenServicioRequest request,
        CancellationToken cancellationToken)
    {
        var (orden, error) = await ordenServicioService.CambiarEstadoAsync(id, request, cancellationToken);

        if (error is not null)
        {
            return NotFound(new { mensaje = error });
        }

        return Ok(orden);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken)
    {
        var eliminada = await ordenServicioService.EliminarAsync(id, cancellationToken);

        if (!eliminada)
        {
            return NotFound(new { mensaje = "Orden de servicio no encontrada." });
        }

        return NoContent();
    }
}
