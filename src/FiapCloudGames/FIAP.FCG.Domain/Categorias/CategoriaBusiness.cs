using FIAP.FCG.Infrastructure.Dados.Repositorios;

namespace FIAP.FCG.Domain.Categorias;

public class CategoriaBusiness : ICategoriaBusiness
{

	private readonly ICategoriaRepository _repositorio;

	public CategoriaBusiness(ICategoriaRepository repositorio)
	{
		_repositorio = repositorio;
	}

	public async Task<bool> NomeJaExiste(string nome, int? excluirId = null)
	{
		return (await _repositorio.ConsultarPorNomeAsync(nome, excluirId)) != null;
	}

}
