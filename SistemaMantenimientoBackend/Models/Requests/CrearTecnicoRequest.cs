using System.ComponentModel.DataAnnotations;
using SistemaMantenimientoBackend.Models.Validation;

namespace SistemaMantenimientoBackend.Models.Requests;

public class CrearTecnicoRequest
{
    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [MinLength(1)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [TelefonoValido]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "La especialidad es obligatoria.")]
    [MinLength(1)]
    public string Especialidad { get; set; } = string.Empty;
}
