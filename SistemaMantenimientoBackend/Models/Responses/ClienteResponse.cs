namespace SistemaMantenimientoBackend.Models.Responses;

public class ClienteResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string DocumentoIdentidad { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
