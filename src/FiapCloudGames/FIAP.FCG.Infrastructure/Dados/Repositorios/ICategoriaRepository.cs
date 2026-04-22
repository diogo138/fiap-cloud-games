using FIAP.FCG.Infrastructure.Dados.Entidades;

namespace FIAP.FCG.Infrastructure.Dados.Repositorios;

public interface ICategoriaRepository
{

	Task<Categoria?> ConsultarAsync(int id);

	Task<IEnumerable<Categoria>> ConsultarTodosAsync(bool? ativo, string? q);

	Task<Categoria?> ConsultarPorNomeAsync(string nome, int? excluirId = null);

	Task AdicionarAsync(Categoria categoria);

	Task AtualizarAsync(Categoria categoria);

}
