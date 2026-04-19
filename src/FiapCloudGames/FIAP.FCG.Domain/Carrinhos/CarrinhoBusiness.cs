using FIAP.FCG.Infrastructure.Dados.Repositorios;

namespace FIAP.FCG.Domain.Carrinhos
{
	public class CarrinhoBusiness : ICarrinhoBusiness
	{

		private readonly ICarrinhoRepository _repositorio;

		public CarrinhoBusiness(ICarrinhoRepository repositorio)
		{
			_repositorio = repositorio;
		}

		public async Task<bool> UsuarioJaPossuiJogo(int usuario, int jogo)
		{
			return (await _repositorio.ConsultarJogoDoUsuarioAsync(usuario, jogo)) != null;
		}

		public async Task<bool> CarrinhoExiste(int id)
		{
			return (await _repositorio.ConsultarAsync(id)) != null;
		}

	}
}
