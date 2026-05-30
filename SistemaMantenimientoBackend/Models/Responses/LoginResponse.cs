namespace SistemaMantenimientoBackend.Models.Responses;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraEn { get; set; }
    public UsuarioResponse Usuario { get; set; } = null!;
}
