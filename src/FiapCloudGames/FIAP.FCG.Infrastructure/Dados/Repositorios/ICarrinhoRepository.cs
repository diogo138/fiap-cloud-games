using FIAP.FCG.Infrastructure.Dados.Entidades;

namespace FIAP.FCG.Infrastructure.Dados.Repositorios;

public interface ICarrinhoRepository
{

	Task<Carrinho?> ConsultarAsync(int id);

	Task<IEnumerable<Carrinho>> ConsultarUsuarioAsync(int usuario);

	Task<Carrinho?> ConsultarJogoDoUsuarioAsync(int usuario, int jogo);

	Task AdicionarAsync(Carrinho carrinho);

	Task AtualizarAsync(Carrinho carrinho);

}
