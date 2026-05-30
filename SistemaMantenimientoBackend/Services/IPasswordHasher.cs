namespace SistemaMantenimientoBackend.Services;

public interface IPasswordHasher
{
    bool Verificar(string contrasena, string hash);
}
