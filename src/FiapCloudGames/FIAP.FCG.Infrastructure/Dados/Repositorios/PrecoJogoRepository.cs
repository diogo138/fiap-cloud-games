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

    public async Task<decimal> ObterHistoricoDePrecoAsync(int jogoId)
    {
        return await _repositorio
            .Where(p => p.JogoId == jogoId)
            .OrderByDescending(p => p.DataInicio)
            .Select(p => p.Valor)
            .FirstOrDefaultAsync();
    }

    public async Task RemoverAsync(int promocaoId, int jogoId)
    {
        var jogo = await _repositorio
            .Where(p => p.PromocaoId == promocaoId && p.JogoId == jogoId)
            .OrderByDescending(p => p.DataInicio)
            .FirstOrDefaultAsync();

        if (jogo == null)
            throw new Exception("Promoção não encontrada para esse jogo");

        jogo.PromocaoId = null;
        jogo.PercDesconto = null;
    }
}
