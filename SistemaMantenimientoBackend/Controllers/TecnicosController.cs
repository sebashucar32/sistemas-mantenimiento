using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaMantenimientoBackend.Models.Requests;
using SistemaMantenimientoBackend.Models.Responses;
using SistemaMantenimientoBackend.Services;

namespace SistemaMantenimientoBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TecnicosController(ITecnicoService tecnicoService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(TecnicoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Crear(
        [FromBody] CrearTecnicoRequest request,
        CancellationToken cancellationToken)
    {
        var (tecnico, error) = await tecnicoService.CrearAsync(request, cancellationToken);

        if (error is not null)
        {
            return BadRequest(new { mensaje = error });
        }

        return CreatedAtAction(nameof(ObtenerPorId), new { id = tecnico!.Id }, tecnico);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TecnicoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorId(int id, CancellationToken cancellationToken)
    {
        var tecnico = await tecnicoService.ObtenerPorIdAsync(id, cancellationToken);

        if (tecnico is null)
        {
            return NotFound(new { mensaje = "Técnico no encontrado." });
        }

        return Ok(tecnico);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TecnicoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
        [FromQuery] bool? activo,
        CancellationToken cancellationToken)
    {
        var tecnicos = await tecnicoService.ListarAsync(activo, cancellationToken);
        return Ok(tecnicos);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TecnicoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] ActualizarTecnicoRequest request,
        CancellationToken cancellationToken)
    {
        var (tecnico, error) = await tecnicoService.ActualizarAsync(id, request, cancellationToken);

        if (error is not null)
        {
            if (error.Contains("no encontrado"))
            {
                return NotFound(new { mensaje = error });
            }

            return BadRequest(new { mensaje = error });
        }

        return Ok(tecnico);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken)
    {
        var eliminado = await tecnicoService.EliminarAsync(id, cancellationToken);

        if (!eliminado)
        {
            return NotFound(new { mensaje = "Técnico no encontrado." });
        }

        return NoContent();
    }
}
