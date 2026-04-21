using FIAP.FCG.Application.Jogos.Dtos;
using FIAP.FCG.Application.Jogos.Services;
using FIAP.FCG.Tests;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace FIAP.FCG.Application.Tests.Jogos.Services;

public class JogoServiceTest : PadraoTest
{

	private IJogoService Service => CarregarInstancia<IJogoService>();

	[Test]
	public async Task ListarAsync_QuandoExistiremJogos_DeveRetornarTodosMapeados()
	{
		var categoria = DataFakeFactory.NovaCategoria();
		var jogoA = DataFakeFactory.NovoJogo("A", categoria);
		var jogoB = DataFakeFactory.NovoJogo("B", categoria);
		await AdicionarAsync(categoria, jogoA, jogoB);

		var resultado = (await Service.ListarAsync(null, null, null, null)).ToList();

		resultado.Count.ShouldBe(2);
	}

	[Test]
	public async Task ListarAsync_QuandoFiltrarPorCategoria_DeveRetornarApenasJogosDaCategoria()
	{
		var categoriaA = DataFakeFactory.NovaCategoria("A");
		var categoriaB = DataFakeFactory.NovaCategoria("B");
		var jogoA = DataFakeFactory.NovoJogo("A", categoriaA);
		var jogoB = DataFakeFactory.NovoJogo("B", categoriaB);
		await AdicionarAsync(categoriaA, categoriaB, jogoA, jogoB);

		var resultado = (await Service.ListarAsync(categoriaA.Id, null, null, null)).ToList();

		resultado.Count.ShouldBe(1);
		resultado[0].CategoriaId.ShouldBe(categoriaA.Id);
	}

	[Test]
	public async Task ConsultarAsync_QuandoJogoExistir_DeveRetornarDto()
	{
		var categoria = DataFakeFactory.NovaCategoria();
		var jogo = DataFakeFactory.NovoJogo("A", categoria);
		await AdicionarAsync(categoria, jogo);

		var resultado = await Service.ConsultarAsync(jogo.Id);

		resultado.Id.ShouldBe(jogo.Id);
		resultado.Nome.ShouldBe(jogo.Nome);
	}

	[Test]
	public async Task ConsultarAsync_QuandoJogoNaoExistir_DeveLancarKeyNotFoundException()
	{
		await Should.ThrowAsync<KeyNotFoundException>(() => Service.ConsultarAsync(999));
	}

	[Test]
	public async Task AdicionarAsync_QuandoNomeDuplicado_DeveLancarInvalidOperationException()
	{
		var categoria = DataFakeFactory.NovaCategoria();
		var jogo = DataFakeFactory.NovoJogo("A", categoria);
		await AdicionarAsync(categoria, jogo);

		var dto = new JogoNovoDto { Nome = jogo.Nome, CategoriaId = categoria.Id };

		var ex = await Should.ThrowAsync<InvalidOperationException>(() => Service.AdicionarAsync(dto));
		ex.Message.ShouldContain("nome");
	}

	[Test]
	public async Task AdicionarAsync_QuandoNomeUnico_DeveAdicionarERetornarDto()
	{
		var categoria = DataFakeFactory.NovaCategoria();
		await AdicionarAsync(categoria);

		var dto = new JogoNovoDto { Nome = "Novo Jogo", CategoriaId = categoria.Id, Visivel = true };

		var resultado = await Service.AdicionarAsync(dto);
		await PersistirAsync();

		resultado.Nome.ShouldBe(dto.Nome);
		resultado.CategoriaId.ShouldBe(dto.CategoriaId);
		resultado.Ativo.ShouldBeTrue();

		var salvo = await Contexto.Jogos.AsNoTracking().SingleAsync(j => j.Nome == dto.Nome);
		salvo.ShouldNotBeNull();
	}

	[Test]
	public async Task AtualizarAsync_QuandoJogoNaoExistir_DeveLancarKeyNotFoundException()
	{
		var dto = new JogoAtualizadoDto { Nome = "Qualquer", CategoriaId = 1, Ativo = true, Visivel = true };

		await Should.ThrowAsync<KeyNotFoundException>(() => Service.AtualizarAsync(999, dto));
	}

	[Test]
	public async Task AtualizarAsync_QuandoDadosValidos_DeveAtualizarERetornarDto()
	{
		var categoria = DataFakeFactory.NovaCategoria();
		var jogo = DataFakeFactory.NovoJogo("A", categoria);
		await AdicionarAsync(categoria, jogo);

		var dto = new JogoAtualizadoDto { Nome = "Jogo Atualizado", CategoriaId = categoria.Id, Visivel = false, Ativo = true };

		var resultado = await Service.AtualizarAsync(jogo.Id, dto);
		await PersistirAsync();

		resultado.Nome.ShouldBe(dto.Nome);
		resultado.Visivel.ShouldBeFalse();
	}

	[Test]
	public async Task RemoverAsync_QuandoJogoExistir_DeveRealizarSoftDelete()
	{
		var categoria = DataFakeFactory.NovaCategoria();
		var jogo = DataFakeFactory.NovoJogo("A", categoria);
		await AdicionarAsync(categoria, jogo);

		await Service.RemoverAsync(jogo.Id);
		await PersistirAsync();

		var salvo = await Contexto.Jogos.AsNoTracking().SingleAsync(j => j.Id == jogo.Id);
		salvo.Ativo.ShouldBeFalse();
	}

	[Test]
	public async Task RemoverAsync_QuandoJogoNaoExistir_DeveLancarKeyNotFoundException()
	{
		await Should.ThrowAsync<KeyNotFoundException>(() => Service.RemoverAsync(999));
	}

	[Test]
	public async Task AjustarPrecoAsync_QuandoJogoExistir_DeveCriarNovoPrecoJogo()
	{
		var categoria = DataFakeFactory.NovaCategoria();
		var jogo = DataFakeFactory.NovoJogo("A", categoria);
		await AdicionarAsync(categoria, jogo);

		var dto = new AjustarPrecoDto { Valor = 49.90m, PercDesconto = null, PromocaoId = null };

		await Service.AjustarPrecoAsync(jogo.Id, dto);
		await PersistirAsync();

		var preco = await Contexto.PrecosJogos.AsNoTracking().SingleAsync(p => p.JogoId == jogo.Id);
		preco.Valor.ShouldBe(dto.Valor);
	}

	[Test]
	public async Task AjustarPrecoAsync_QuandoJogoNaoExistir_DeveLancarKeyNotFoundException()
	{
		var dto = new AjustarPrecoDto { Valor = 49.90m };

		await Should.ThrowAsync<KeyNotFoundException>(() => Service.AjustarPrecoAsync(999, dto));
	}

}
