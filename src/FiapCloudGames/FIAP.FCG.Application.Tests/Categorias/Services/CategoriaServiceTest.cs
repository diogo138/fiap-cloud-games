using FIAP.FCG.Application.Categorias.Dtos;
using FIAP.FCG.Application.Categorias.Services;
using FIAP.FCG.Tests;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace FIAP.FCG.Application.Tests.Categorias.Services;

public class CategoriaServiceTest : PadraoTest
{

	private ICategoriaService Service => CarregarInstancia<ICategoriaService>();

	[Test]
	public async Task ListarAsync_QuandoExistiremCategorias_DeveRetornarTodasMapeadas()
	{
		var categoriaA = DataFakeFactory.NovaCategoria("A");
		var categoriaB = DataFakeFactory.NovaCategoria("B");
		await AdicionarAsync(categoriaA, categoriaB);

		var resultado = (await Service.ListarAsync(null, null)).ToList();

		resultado.Count.ShouldBe(2);
	}

	[Test]
	public async Task ListarAsync_QuandoFiltrarPorAtivo_DeveRetornarApenasAtivas()
	{
		var categoriaAtiva = DataFakeFactory.NovaCategoria("A");
		var categoriaInativa = DataFakeFactory.NovaCategoria("B");
		categoriaInativa.Ativo = false;
		await AdicionarAsync(categoriaAtiva, categoriaInativa);

		var resultado = (await Service.ListarAsync(ativo: true, null)).ToList();

		resultado.Count.ShouldBe(1);
		resultado[0].Ativo.ShouldBeTrue();
	}

	[Test]
	public async Task ConsultarAsync_QuandoCategoriaExistir_DeveRetornarDto()
	{
		var categoria = DataFakeFactory.NovaCategoria("A");
		await AdicionarAsync(categoria);

		var resultado = await Service.ConsultarAsync(categoria.Id);

		resultado.Id.ShouldBe(categoria.Id);
		resultado.Nome.ShouldBe(categoria.Nome);
	}

	[Test]
	public async Task ConsultarAsync_QuandoCategoriaNaoExistir_DeveLancarKeyNotFoundException()
	{
		await Should.ThrowAsync<KeyNotFoundException>(() => Service.ConsultarAsync(999));
	}

	[Test]
	public async Task AdicionarAsync_QuandoNomeDuplicado_DeveLancarInvalidOperationException()
	{
		var categoria = DataFakeFactory.NovaCategoria("A");
		await AdicionarAsync(categoria);

		var dto = new CategoriaNovoDto { Nome = categoria.Nome };

		var ex = await Should.ThrowAsync<InvalidOperationException>(() => Service.AdicionarAsync(dto));
		ex.Message.ShouldContain("nome");
	}

	[Test]
	public async Task AdicionarAsync_QuandoNomeUnico_DeveAdicionarERetornarDto()
	{
		var dto = new CategoriaNovoDto { Nome = "RPG", Descricao = "Jogos de RPG" };

		var resultado = await Service.AdicionarAsync(dto);
		await PersistirAsync();

		resultado.Nome.ShouldBe(dto.Nome);
		resultado.Descricao.ShouldBe(dto.Descricao);
		resultado.Ativo.ShouldBeTrue();

		var salvo = await Contexto.Categorias.AsNoTracking().SingleAsync(c => c.Nome == dto.Nome);
		salvo.ShouldNotBeNull();
	}

	[Test]
	public async Task AtualizarAsync_QuandoCategoriaNaoExistir_DeveLancarKeyNotFoundException()
	{
		var dto = new CategoriaAtualizadoDto { Nome = "Novo Nome", Ativo = true };

		await Should.ThrowAsync<KeyNotFoundException>(() => Service.AtualizarAsync(999, dto));
	}

	[Test]
	public async Task AtualizarAsync_QuandoNomeDuplicadoDeOutroRegistro_DeveLancarInvalidOperationException()
	{
		var categoriaA = DataFakeFactory.NovaCategoria("A");
		var categoriaB = DataFakeFactory.NovaCategoria("B");
		await AdicionarAsync(categoriaA, categoriaB);

		var dto = new CategoriaAtualizadoDto { Nome = categoriaB.Nome, Ativo = true };

		var ex = await Should.ThrowAsync<InvalidOperationException>(() => Service.AtualizarAsync(categoriaA.Id, dto));
		ex.Message.ShouldContain("nome");
	}

	[Test]
	public async Task AtualizarAsync_QuandoDadosValidos_DeveAtualizarERetornarDto()
	{
		var categoria = DataFakeFactory.NovaCategoria("A");
		await AdicionarAsync(categoria);

		var dto = new CategoriaAtualizadoDto { Nome = "RPG Atualizado", Descricao = "Desc nova", Ativo = true };

		var resultado = await Service.AtualizarAsync(categoria.Id, dto);
		await PersistirAsync();

		resultado.Nome.ShouldBe(dto.Nome);
		resultado.Descricao.ShouldBe(dto.Descricao);
	}

	[Test]
	public async Task RemoverAsync_QuandoCategoriaExistir_DeveRealizarSoftDelete()
	{
		var categoria = DataFakeFactory.NovaCategoria("A");
		await AdicionarAsync(categoria);

		await Service.RemoverAsync(categoria.Id);
		await PersistirAsync();

		var salvo = await Contexto.Categorias.AsNoTracking().SingleAsync(c => c.Id == categoria.Id);
		salvo.Ativo.ShouldBeFalse();
	}

	[Test]
	public async Task RemoverAsync_QuandoCategoriaNaoExistir_DeveLancarKeyNotFoundException()
	{
		await Should.ThrowAsync<KeyNotFoundException>(() => Service.RemoverAsync(999));
	}

}
