namespace FIAP.FCG.Domain.Categorias;

public interface ICategoriaBusiness
{

	Task<bool> NomeJaExiste(string nome, int? excluirId = null);

}
