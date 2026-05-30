namespace SistemaMantenimientoBackend.Models;

public class Usuario
{
    public int Id { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string HashContrasena { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string? CorreoElectronico { get; set; }
    public bool Activo { get; set; }
}
