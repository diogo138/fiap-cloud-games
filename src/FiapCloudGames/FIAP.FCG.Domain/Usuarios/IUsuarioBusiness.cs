namespace FIAP.FCG.Domain.Usuarios;

public interface IUsuarioBusiness
{
    Task<bool> EmailJaCadastrado(string email);
    Task<bool> UsuarioExiste(int id);
}
