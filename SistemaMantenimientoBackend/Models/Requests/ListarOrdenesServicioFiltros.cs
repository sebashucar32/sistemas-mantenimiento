using SistemaMantenimientoBackend.Models.Enums;

namespace SistemaMantenimientoBackend.Models.Requests;

public class ListarOrdenesServicioFiltros
{
    public EstadoOrdenServicio? Estado { get; set; }

    public int? ClienteId { get; set; }

    public int? TecnicoId { get; set; }

    /// <summary>Busca por nombre o especialidad del técnico (coincidencia parcial).</summary>
    public string? TecnicoBusqueda { get; set; }

    /// <summary>Busca por nombre o documento de identidad del cliente (coincidencia parcial).</summary>
    public string? ClienteBusqueda { get; set; }

    public DateTime? FechaCreacionDesde { get; set; }

    public DateTime? FechaCreacionHasta { get; set; }
}
