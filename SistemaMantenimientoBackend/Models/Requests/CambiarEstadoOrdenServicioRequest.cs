using System.ComponentModel.DataAnnotations;
using SistemaMantenimientoBackend.Models.Enums;

namespace SistemaMantenimientoBackend.Models.Requests;

public class CambiarEstadoOrdenServicioRequest
{
    [Required]
    public EstadoOrdenServicio Estado { get; set; }
}
