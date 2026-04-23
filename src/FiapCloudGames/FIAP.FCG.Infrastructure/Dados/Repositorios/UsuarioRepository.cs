using FIAP.FCG.Infrastructure.Dados.Entidades;
using Microsoft.EntityFrameworkCore;

namespace FIAP.FCG.Infrastructure.Dados.Repositorios;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly DbSet<Usuario> _repositorio;

    public UsuarioRepository(IUnidadeDeTrabalho unidade)
    {
        _repositorio = unidade.Repositorio<Usuario>();
    }

    public async Task<Usuario?> ConsultarAsync(int id)
        => await _repositorio.FindAsync(id);

    public async Task<Usuario?> ConsultarPorEmailAsync(string email)
        => await _repositorio.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<IEnumerable<Usuario>> ListarAsync()
        => await _repositorio.ToListAsync();

    public async Task AdicionarAsync(Usuario usuario)
        => await _repositorio.AddAsync(usuario);

    public async Task AtualizarAsync(Usuario usuario)
    {
        _repositorio.Update(usuario);
        await Task.CompletedTask;
    }

    public async Task<Usuario?> ConsultarBibliotecaAsync(int id)
    {
        var usuario = await _repositorio
            .Include(u => u.JogosAdquiridos)
                .ThenInclude(b => b.Jogo)
                    .ThenInclude(b => b.Categoria)
			.FirstOrDefaultAsync(u => u.Id == id);
        return usuario;
    }

}
