using FIAP.FCG.Infrastructure.Dados.Entidades;

namespace FIAP.FCG.Infrastructure.Dados.Repositorios;

public interface IUsuarioRepository
{
    Task<Usuario?> ConsultarAsync(int id);
    Task<Usuario?> ConsultarPorEmailAsync(string email);
    Task<IEnumerable<Usuario>> ListarAsync();
    Task AdicionarAsync(Usuario usuario);
    Task AtualizarAsync(Usuario usuario);
    Task<Usuario?> ConsultarBibliotecaAsync(int id);
}
