using FIAP.FCG.Infrastructure.Dados.Entidades;

namespace FIAP.FCG.Infrastructure.Dados.Repositorios;

public interface IJogoRepository
{

	Task<Jogo?> ConsultarAsync(int id);

	Task<IEnumerable<Jogo>> ConsultarTodosAsync(int? categoriaId, bool? visivel, bool? ativo, string? q);

	Task<Jogo?> ConsultarPorNomeAsync(string nome, int? excluirId = null);

	Task AdicionarAsync(Jogo jogo);

	Task AtualizarAsync(Jogo jogo);

}
