using FIAP.FCG.Infrastructure.Dados.Entidades;
using Microsoft.EntityFrameworkCore;

namespace FIAP.FCG.Infrastructure.Dados.Repositorios;

public class CategoriaRepository : ICategoriaRepository
{

	private readonly DbSet<Categoria> _repositorio;

	public CategoriaRepository(IUnidadeDeTrabalho unidade)
	{
		_repositorio = unidade.Repositorio<Categoria>();
	}

	public async Task<Categoria?> ConsultarAsync(int id)
		=> await _repositorio.FindAsync(id);

	public async Task<IEnumerable<Categoria>> ConsultarTodosAsync(bool? ativo, string? q)
	{
		var query = _repositorio.AsQueryable();

		if (ativo.HasValue)
			query = query.Where(c => c.Ativo == ativo.Value);

		if (!string.IsNullOrWhiteSpace(q))
			query = query.Where(c => c.Nome.Contains(q) || (c.Descricao != null && c.Descricao.Contains(q)));

		return await query.OrderBy(c => c.Nome).ToListAsync();
	}

	public async Task<Categoria?> ConsultarPorNomeAsync(string nome, int? excluirId = null)
	{
		var query = _repositorio.Where(c => c.Nome == nome);

		if (excluirId.HasValue)
			query = query.Where(c => c.Id != excluirId.Value);

		return await query.FirstOrDefaultAsync();
	}

	public async Task AdicionarAsync(Categoria categoria)
	{
		await _repositorio.AddAsync(categoria);
	}

	public async Task AtualizarAsync(Categoria categoria)
	{
		_repositorio.Update(categoria);
		await Task.CompletedTask;
	}

}
