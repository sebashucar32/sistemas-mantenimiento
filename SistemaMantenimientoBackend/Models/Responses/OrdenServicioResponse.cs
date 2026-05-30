using SistemaMantenimientoBackend.Models.Enums;

namespace SistemaMantenimientoBackend.Models.Responses;

public class OrdenServicioResponse
{
    public int Id { get; set; }
    public DateTime FechaCreacion { get; set; }
    public EstadoOrdenServicio Estado { get; set; }
    public string EstadoDescripcion { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int TecnicoId { get; set; }
    public string TecnicoNombre { get; set; } = string.Empty;
    public string TecnicoEspecialidad { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public string ClienteDocumentoIdentidad { get; set; } = string.Empty;
}
