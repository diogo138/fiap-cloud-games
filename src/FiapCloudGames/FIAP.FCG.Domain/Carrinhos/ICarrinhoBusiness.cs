
namespace FIAP.FCG.Domain.Carrinhos
{
	public interface ICarrinhoBusiness
	{

		Task<bool> UsuarioJaPossuiJogo(int usuario, int jogo);

		Task<bool> CarrinhoExiste(int id);

	}
}