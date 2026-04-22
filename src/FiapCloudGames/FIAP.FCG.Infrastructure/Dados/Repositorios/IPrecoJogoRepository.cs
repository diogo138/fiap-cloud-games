using FIAP.FCG.Infrastructure.Dados.Entidades;

namespace FIAP.FCG.Infrastructure.Dados.Repositorios;

public interface IPrecoJogoRepository
{

	Task<IEnumerable<PrecoJogo>> ConsultarPorJogoAsync(int jogoId);

	Task AdicionarAsync(PrecoJogo precoJogo);

}
