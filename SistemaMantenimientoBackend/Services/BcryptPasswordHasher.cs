namespace SistemaMantenimientoBackend.Services;

public class BcryptPasswordHasher : IPasswordHasher
{
    public bool Verificar(string contrasena, string hash) =>
        BCrypt.Net.BCrypt.Verify(contrasena, hash);
}
