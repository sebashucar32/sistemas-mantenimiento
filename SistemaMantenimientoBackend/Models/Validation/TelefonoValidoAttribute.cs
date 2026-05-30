using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SistemaMantenimientoBackend.Models.Validation;

public partial class TelefonoValidoAttribute : ValidationAttribute
{
    [GeneratedRegex(@"^\+?[0-9]{7,15}$")]
    private static partial Regex TelefonoRegex();

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null or "")
        {
            return ValidationResult.Success;
        }

        var telefono = value.ToString()!.Trim();

        if (!TelefonoRegex().IsMatch(telefono))
        {
            return new ValidationResult("El teléfono debe contener entre 7 y 15 dígitos numéricos, con un '+' opcional al inicio.");
        }

        return ValidationResult.Success;
    }
}
