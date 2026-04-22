using FIAP.FCG.Infrastructure.Dados.Entidades;
using Microsoft.EntityFrameworkCore;

namespace FIAP.FCG.Infrastructure.Dados.Repositorios;

public class JogoRepository : IJogoRepository
{

	private readonly DbSet<Jogo> _repositorio;

	public JogoRepository(IUnidadeDeTrabalho unidade)
	{
		_repositorio = unidade.Repositorio<Jogo>();
	}

	public async Task<Jogo?> ConsultarAsync(int id)
		=> await _repositorio.FindAsync(id);

	public async Task<IEnumerable<Jogo>> ConsultarTodosAsync(int? categoriaId, bool? visivel, bool? ativo, string? q)
	{
		var query = _repositorio.AsQueryable();

		if (categoriaId.HasValue)
			query = query.Where(j => j.CategoriaId == categoriaId.Value);

		if (visivel.HasValue)
			query = query.Where(j => j.Visivel == visivel.Value);

		if (ativo.HasValue)
			query = query.Where(j => j.Ativo == ativo.Value);

		if (!string.IsNullOrWhiteSpace(q))
			query = query.Where(j => j.Nome.Contains(q) || (j.Descricao != null && j.Descricao.Contains(q)));

		return await query.OrderBy(j => j.Nome).ToListAsync();
	}

	public async Task<Jogo?> ConsultarPorNomeAsync(string nome, int? excluirId = null)
	{
		var query = _repositorio.Where(j => j.Nome == nome);

		if (excluirId.HasValue)
			query = query.Where(j => j.Id != excluirId.Value);

		return await query.FirstOrDefaultAsync();
	}

	public async Task AdicionarAsync(Jogo jogo)
	{
		await _repositorio.AddAsync(jogo);
	}

	public async Task AtualizarAsync(Jogo jogo)
	{
		_repositorio.Update(jogo);
		await Task.CompletedTask;
	}

}
