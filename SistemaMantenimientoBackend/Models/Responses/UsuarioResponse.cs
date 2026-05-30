namespace SistemaMantenimientoBackend.Models.Responses;

public class UsuarioResponse
{
    public int Id { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string? CorreoElectronico { get; set; }
}
