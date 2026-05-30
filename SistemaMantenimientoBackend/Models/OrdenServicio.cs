namespace SistemaMantenimientoBackend.Models;

public class OrdenServicio
{
    public int Id { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int TecnicoId { get; set; }
    public int ClienteId { get; set; }
    public string? TecnicoNombre { get; set; }
    public string? TecnicoEspecialidad { get; set; }
    public string? ClienteNombre { get; set; }
    public string? ClienteDocumentoIdentidad { get; set; }
}
