using SistemaMantenimientoBackend.Models;

namespace SistemaMantenimientoBackend.Services;

public interface ITokenService
{
    (string Token, DateTime ExpiraEn) GenerarToken(Usuario usuario);
}
