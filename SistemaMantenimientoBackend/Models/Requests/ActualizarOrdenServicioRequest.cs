using System.ComponentModel.DataAnnotations;
using SistemaMantenimientoBackend.Models.Enums;

namespace SistemaMantenimientoBackend.Models.Requests;

public class ActualizarOrdenServicioRequest
{
    [Required]
    [MinLength(1)]
    public string Descripcion { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int TecnicoId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int ClienteId { get; set; }

    [Required]
    public EstadoOrdenServicio Estado { get; set; }
}
