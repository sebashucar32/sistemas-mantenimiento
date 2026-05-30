using System.ComponentModel.DataAnnotations;
using SistemaMantenimientoBackend.Models.Validation;

namespace SistemaMantenimientoBackend.Models.Requests;

public class ActualizarClienteRequest
{
    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [MinLength(1)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El documento de identidad es obligatorio.")]
    [MinLength(1)]
    public string DocumentoIdentidad { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección es obligatoria.")]
    [MinLength(1)]
    public string Direccion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [TelefonoValido]
    public string Telefono { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;
}
