using FIAP.FCG.Domain.Carrinhos;
using FIAP.FCG.Infrastructure.Dados.Entidades;
using FIAP.FCG.Tests;
using Shouldly;

namespace FIAP.FCG.Domain.Tests.Carrinhos;

public class CarrinhoBusinessTest : PadraoTest
{

	private ICarrinhoBusiness Business => CarregarInstancia<ICarrinhoBusiness>();

	[Test]
	public async Task UsuarioJaPossuiJogo_QuandoCarrinhoDoUsuarioConterJogo_DeveRetornarTrue()
	{
		var categoria = DataFakeFactory.NovaCategoria();
		var usuario = DataFakeFactory.NovoUsuario("A");
		var jogo = DataFakeFactory.NovoJogo("A", categoria);
		var carrinho = DataFakeFactory.NovoCarrinho(jogo, usuario);

		await AdicionarAsync(categoria, usuario, jogo, carrinho);

		var resultado = await Business.UsuarioJaPossuiJogo(usuario.Id, jogo.Id);

		resultado.ShouldBeTrue();
	}

	[Test]
	public async Task UsuarioJaPossuiJogo_QuandoCarrinhoDoUsuarioNaoConterJogo_DeveRetornarFalse()
	{
		var categoria = DataFakeFactory.NovaCategoria();
		var usuario = DataFakeFactory.NovoUsuario("A");
		var jogo = DataFakeFactory.NovoJogo("A", categoria);

		await AdicionarAsync(categoria, usuario, jogo);

		var resultado = await Business.UsuarioJaPossuiJogo(usuario.Id, jogo.Id);

		resultado.ShouldBeFalse();
	}

	[Test]
	public async Task CarrinhoExiste_QuandoCarrinhoExistir_DeveRetornarTrue()
	{
		var categoria = DataFakeFactory.NovaCategoria();
		var usuario = DataFakeFactory.NovoUsuario("A");
		var jogo = DataFakeFactory.NovoJogo("A", categoria);
		var carrinho = DataFakeFactory.NovoCarrinho(jogo, usuario);

		await AdicionarAsync(categoria, usuario, jogo, carrinho);

		var resultado = await Business.CarrinhoExiste(carrinho.Id);

		resultado.ShouldBeTrue();
	}

	[Test]
	public async Task CarrinhoExiste_QuandoCarrinhoNaoExistir_DeveRetornarFalse()
	{
		var resultado = await Business.CarrinhoExiste(999);

		resultado.ShouldBeFalse();
	}
}
