using FIAP.FCG.Infrastructure.Dados.Entidades;
using Microsoft.EntityFrameworkCore;

namespace FIAP.FCG.Infrastructure.Dados.Repositorios;

public class CarrinhoRepository : ICarrinhoRepository
{

	private readonly DbSet<Carrinho> _repositorio;

	public CarrinhoRepository(IUnidadeDeTrabalho unidade)
	{
		_repositorio = unidade.Repositorio<Carrinho>();
	}

	public async Task<Carrinho?> ConsultarAsync(int id)
		=> await _repositorio.FindAsync(id);

	public async Task<IEnumerable<Carrinho>> ConsultarUsuarioAsync(int usuarioId)
		=> await _repositorio.Where(c => c.UsuarioId == usuarioId)
			.ToListAsync();

	public async Task<Carrinho?> ConsultarUsuarioCarrinhoAsync(int usuarioId, int carrinhoId)
		=> await _repositorio.Where(c => c.UsuarioId == usuarioId && c.Id == carrinhoId)
			.FirstOrDefaultAsync();

	public async Task<Carrinho?> ConsultarJogoDoUsuarioAsync(int usuarioId, int jogoId)
		=> await _repositorio.FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.JogoId == jogoId);

	public async Task AdicionarAsync(Carrinho carrinho)
	{
		await _repositorio.AddAsync(carrinho);
	}

	public async Task AtualizarAsync(Carrinho carrinho)
	{
		_repositorio.Update(carrinho);
		await Task.CompletedTask;
	}

	public async Task RemoverAsync(Carrinho carrinho)
	{
		_repositorio.Remove(carrinho);
		await Task.CompletedTask;
	}

}
