using SistemaMantenimientoBackend.Models.Enums;

namespace SistemaMantenimientoBackend.Models;

public static class EstadoOrdenServicioMapper
{
    public static string AValorBaseDatos(EstadoOrdenServicio estado) => estado switch
    {
        EstadoOrdenServicio.Pendiente => "pendiente",
        EstadoOrdenServicio.EnProgreso => "en_progreso",
        EstadoOrdenServicio.Finalizada => "finalizada",
        _ => throw new ArgumentOutOfRangeException(nameof(estado), estado, "Estado de orden no válido.")
    };

    public static EstadoOrdenServicio DesdeValorBaseDatos(string valor) => valor switch
    {
        "pendiente" => EstadoOrdenServicio.Pendiente,
        "en_progreso" => EstadoOrdenServicio.EnProgreso,
        "finalizada" => EstadoOrdenServicio.Finalizada,
        _ => throw new ArgumentOutOfRangeException(nameof(valor), valor, "Estado de orden no válido.")
    };

    public static string AEtiqueta(EstadoOrdenServicio estado) => estado switch
    {
        EstadoOrdenServicio.Pendiente => "Pendiente",
        EstadoOrdenServicio.EnProgreso => "En progreso",
        EstadoOrdenServicio.Finalizada => "Finalizada",
        _ => throw new ArgumentOutOfRangeException(nameof(estado), estado, "Estado de orden no válido.")
    };
}
