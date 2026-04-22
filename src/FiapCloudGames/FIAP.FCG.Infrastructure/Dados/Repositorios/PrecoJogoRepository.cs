using FIAP.FCG.Infrastructure.Dados.Entidades;
using Microsoft.EntityFrameworkCore;

namespace FIAP.FCG.Infrastructure.Dados.Repositorios;

public class PrecoJogoRepository : IPrecoJogoRepository
{

	private readonly DbSet<PrecoJogo> _repositorio;

	public PrecoJogoRepository(IUnidadeDeTrabalho unidade)
	{
		_repositorio = unidade.Repositorio<PrecoJogo>();
	}

	public async Task<IEnumerable<PrecoJogo>> ConsultarPorJogoAsync(int jogoId)
		=> await _repositorio
			.Where(p => p.JogoId == jogoId)
			.OrderByDescending(p => p.DataInicio)
			.ToListAsync();

	public async Task AdicionarAsync(PrecoJogo precoJogo)
	{
		await _repositorio.AddAsync(precoJogo);
	}

}
