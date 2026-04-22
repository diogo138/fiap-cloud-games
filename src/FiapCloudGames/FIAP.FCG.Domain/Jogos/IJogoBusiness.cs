namespace FIAP.FCG.Domain.Jogos;

public interface IJogoBusiness
{

	Task<bool> NomeJaExiste(string nome, int? excluirId = null);

}
