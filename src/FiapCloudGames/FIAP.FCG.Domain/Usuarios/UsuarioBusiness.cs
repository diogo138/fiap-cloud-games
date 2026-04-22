using FIAP.FCG.Infrastructure.Dados.Repositorios;

namespace FIAP.FCG.Domain.Usuarios;

public class UsuarioBusiness : IUsuarioBusiness
{
    private readonly IUsuarioRepository _repositorio;

    public UsuarioBusiness(IUsuarioRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<bool> EmailJaCadastrado(string email)
        => (await _repositorio.ConsultarPorEmailAsync(email)) != null;

    public async Task<bool> UsuarioExiste(int id)
        => (await _repositorio.ConsultarAsync(id)) != null;
}
