using FIAP.FCG.Application.Carrinhos.Dtos;
using FIAP.FCG.Application.Carrinhos.Services;
using FIAP.FCG.Tests;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace FIAP.FCG.Application.Tests.Carrinhos.Services;

public class CarrinhoServiceTest : PadraoTest
{

	private ICarrinhoService Service => CarregarInstancia<ICarrinhoService>();

	[Test]
	public async Task ListarPorUsuarioAsync_QuandoExistiremCarrinhos_DeveRetornarApenasDoUsuarioMapeado()
	{
		var categoria = DataFakeFactory.NovaCategoria();
		var usuarioA = DataFakeFactory.NovoUsuario("A");
		var usuarioB = DataFakeFactory.NovoUsuario("B");
		var jogoA = DataFakeFactory.NovoJogo("A", categoria);
		var jogoB = DataFakeFactory.NovoJogo("B", categoria);
		var dataCadastroA = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		var dataCadastroB = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc);

		var carrinhoA = DataFakeFactory.NovoCarrinho(jogoA, usuarioA);
		carrinhoA.Quantidade = 2;
		carrinhoA.DataCadastro = dataCadastroA;

		var carrinhoB = DataFakeFactory.NovoCarrinho(jogoB, usuarioB);
		carrinhoB.Quantidade = 1;
		carrinhoB.DataCadastro = dataCadastroB;

		await AdicionarAsync(categoria, usuarioA, usuarioB, jogoA, jogoB, carrinhoA, carrinhoB);

		var resultado = (await Service.ListarPorUsuarioAsync(usuarioA.Id)).ToList();

		resultado.Count.ShouldBe(1);
		resultado[0].Id.ShouldBe(carrinhoA.Id);
		resultado[0].UsuarioId.ShouldBe(carrinhoA.UsuarioId);
		resultado[0].JogoId.ShouldBe(carrinhoA.JogoId);
		resultado[0].Quantidade.ShouldBe(carrinhoA.Quantidade);
		resultado[0].DataCadastro.ShouldBe(carrinhoA.DataCadastro);
	}

	[Test]
	public async Task AdicionarAsync_QuandoUsuarioJaPossuiJogo_DeveLancarInvalidOperationException()
	{
		var categoria = DataFakeFactory.NovaCategoria();
		var usuario = DataFakeFactory.NovoUsuario("A");
		var jogo = DataFakeFactory.NovoJogo("A", categoria);
		var carrinhoExistente = DataFakeFactory.NovoCarrinho(jogo, usuario);

		await AdicionarAsync(categoria, usuario, jogo, carrinhoExistente);

		var dto = new CarrinhoNovoDto { UsuarioId = usuario.Id, JogoId = jogo.Id, Quantidade = 1 };

		var ex = await Should.ThrowAsync<InvalidOperationException>(() => Service.AdicionarAsync(dto));
		ex.Message.ShouldContain("carrinho");
	}

	[Test]
	public async Task AdicionarAsync_QuandoNaoExistirJogoNoCarrinho_DeveAdicionarERetornarDto()
	{
		var categoria = DataFakeFactory.NovaCategoria();
		var usuario = DataFakeFactory.NovoUsuario("A");
		var jogo = DataFakeFactory.NovoJogo("A", categoria);
		await AdicionarAsync(categoria, usuario, jogo);

		var dto = new CarrinhoNovoDto { UsuarioId = usuario.Id, JogoId = jogo.Id, Quantidade = 3 };

		var agora = DateTime.UtcNow;
		var resultado = await Service.AdicionarAsync(dto);
		await PersistirAsync();

		resultado.UsuarioId.ShouldBe(dto.UsuarioId);
		resultado.JogoId.ShouldBe(dto.JogoId);
		resultado.Quantidade.ShouldBe(dto.Quantidade);
		resultado.DataCadastro.ShouldBeInRange(agora.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));

		var salvo = await Contexto.Carrinhos.AsNoTracking().SingleAsync(c =>
			c.UsuarioId == dto.UsuarioId && c.JogoId == dto.JogoId);

		salvo.Quantidade.ShouldBe(dto.Quantidade);
		salvo.DataCadastro.ShouldBe(resultado.DataCadastro);
	}

	[Test]
	public async Task AtualizarQuantidadeAsync_QuandoCarrinhoExistir_DeveAtualizarERetornarDto()
	{
		var categoria = DataFakeFactory.NovaCategoria();
		var usuario = DataFakeFactory.NovoUsuario("A");
		var jogo = DataFakeFactory.NovoJogo("A", categoria);
		var carrinho = DataFakeFactory.NovoCarrinho(jogo, usuario);

		await AdicionarAsync(categoria, usuario, jogo, carrinho);

		var dto = new CarrinhoAtualizadoDto { Quantidade = 5 };

		var resultado = await Service.AtualizarQuantidadeAsync(carrinho.Id, dto);
		await PersistirAsync();

		resultado.Id.ShouldBe(carrinho.Id);
		resultado.UsuarioId.ShouldBe(carrinho.UsuarioId);
		resultado.JogoId.ShouldBe(carrinho.JogoId);
		resultado.Quantidade.ShouldBe(dto.Quantidade);
		resultado.DataCadastro.ShouldBe(carrinho.DataCadastro);

		var atualizado = await Contexto.Carrinhos.AsNoTracking().SingleAsync(c => c.Id == carrinho.Id);
		atualizado.Quantidade.ShouldBe(dto.Quantidade);
	}

	[Test]
	public async Task AtualizarQuantidadeAsync_QuandoCarrinhoNaoExistir_DeveLancarKeyNotFoundException()
	{
		var dto = new CarrinhoAtualizadoDto { Quantidade = 5 };

		var ex = await Should.ThrowAsync<KeyNotFoundException>(() => Service.AtualizarQuantidadeAsync(999, dto));
		ex.Message.ShouldContain("Carrinho");
	}

}
