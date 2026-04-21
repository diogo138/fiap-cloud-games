using FIAP.FCG.Infrastructure.Dados.Repositorios;

namespace FIAP.FCG.Domain.Jogos;

public class JogoBusiness : IJogoBusiness
{

	private readonly IJogoRepository _repositorio;

	public JogoBusiness(IJogoRepository repositorio)
	{
		_repositorio = repositorio;
	}

	public async Task<bool> NomeJaExiste(string nome, int? excluirId = null)
	{
		return (await _repositorio.ConsultarPorNomeAsync(nome, excluirId)) != null;
	}

}
