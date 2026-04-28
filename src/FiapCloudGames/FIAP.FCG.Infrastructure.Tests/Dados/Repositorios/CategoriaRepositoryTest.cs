using FIAP.FCG.Infrastructure.Dados.Repositorios;
using FIAP.FCG.Tests;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace FIAP.FCG.Infrastructure.Tests.Dados.Repositorios;

public class CategoriaRepositoryTest : PadraoTest
{

	private ICategoriaRepository Repositorio => CarregarInstancia<ICategoriaRepository>();

	[Test]
	public async Task ConsultarAsync_QuandoCategoriaExistir_DeveRetornarCategoria()
	{
		var categoria = DataFakeFactory.NovaCategoria("A");
		await AdicionarAsync(categoria);

		var resultado = await Repositorio.ConsultarAsync(categoria.Id);

		resultado.ShouldNotBeNull();
		resultado.Id.ShouldBe(categoria.Id);
		resultado.Nome.ShouldBe(categoria.Nome);
	}

	[Test]
	public async Task ConsultarAsync_QuandoCategoriaNaoExistir_DeveRetornarNull()
	{
		var resultado = await Repositorio.ConsultarAsync(999);

		resultado.ShouldBeNull();
	}

	[Test]
	public async Task ConsultarTodosAsync_QuandoSemFiltros_DeveRetornarCategoriasOrdenadasPorNome()
	{
		var categoriaB = DataFakeFactory.NovaCategoria("B");
		var categoriaA = DataFakeFactory.NovaCategoria("A");
		await AdicionarAsync(categoriaB, categoriaA);

		var resultado = (await Repositorio.ConsultarTodosAsync(null, null)).ToList();

		resultado.Count.ShouldBe(2);
		resultado.Select(c => c.Nome).ShouldBe(["Categoria A", "Categoria B"]);
	}

	[Test]
	public async Task ConsultarTodosAsync_QuandoFiltrarPorAtivo_DeveRetornarApenasCategoriasAtivas()
	{
		var categoriaAtiva = DataFakeFactory.NovaCategoria("A");
		var categoriaInativa = DataFakeFactory.NovaCategoria("B");
		categoriaInativa.Ativo = false;
		await AdicionarAsync(categoriaAtiva, categoriaInativa);

		var resultado = (await Repositorio.ConsultarTodosAsync(ativo: true, q: null)).ToList();

		resultado.Count.ShouldBe(1);
		resultado[0].Id.ShouldBe(categoriaAtiva.Id);
		resultado[0].Ativo.ShouldBeTrue();
	}

	[Test]
	public async Task ConsultarTodosAsync_QuandoFiltrarPorTexto_DevePesquisarEmNomeEDescricao()
	{
		var categoriaNome = DataFakeFactory.NovaCategoria("A");
		categoriaNome.Nome = "Aventura";
		categoriaNome.Descricao = "Categoria principal";

		var categoriaDescricao = DataFakeFactory.NovaCategoria("B");
		categoriaDescricao.Nome = "RPG";
		categoriaDescricao.Descricao = "Jogos de aventura com historia";

		var categoriaSemCorrespondencia = DataFakeFactory.NovaCategoria("C");
		categoriaSemCorrespondencia.Nome = "Corrida";
		categoriaSemCorrespondencia.Descricao = "Velocidade";

		await AdicionarAsync(categoriaNome, categoriaDescricao, categoriaSemCorrespondencia);

		var resultado = (await Repositorio.ConsultarTodosAsync(ativo: null, q: "aventura")).ToList();

		resultado.Count.ShouldBe(2);
		resultado.Any(c => c.Id == categoriaNome.Id).ShouldBeTrue();
		resultado.Any(c => c.Id == categoriaDescricao.Id).ShouldBeTrue();
	}

	[Test]
	public async Task ConsultarPorNomeAsync_QuandoNomeExistir_DeveRetornarCategoria()
	{
		var categoria = DataFakeFactory.NovaCategoria("A");
		await AdicionarAsync(categoria);

		var resultado = await Repositorio.ConsultarPorNomeAsync(categoria.Nome);

		resultado.ShouldNotBeNull();
		resultado.Id.ShouldBe(categoria.Id);
	}

	[Test]
	public async Task ConsultarPorNomeAsync_QuandoExcluirIdForDoMesmoRegistro_DeveRetornarNull()
	{
		var categoria = DataFakeFactory.NovaCategoria("A");
		await AdicionarAsync(categoria);

		var resultado = await Repositorio.ConsultarPorNomeAsync(categoria.Nome, excluirId: categoria.Id);

		resultado.ShouldBeNull();
	}

	[Test]
	public async Task AdicionarAsync_QuandoCategoriaForValida_DeveAdicionarNoBanco()
	{
		var categoria = DataFakeFactory.NovaCategoria("A");
		categoria.Descricao = "Descricao de teste";

		await Repositorio.AdicionarAsync(categoria);
		await PersistirAsync();

		var salvo = await Contexto.Categorias.AsNoTracking().SingleAsync(c => c.Nome == categoria.Nome);

		salvo.ShouldNotBeNull();
		salvo.Descricao.ShouldBe(categoria.Descricao);
	}

	[Test]
	public async Task AtualizarAsync_QuandoCategoriaExistir_DevePersistirAlteracoes()
	{
		var categoria = DataFakeFactory.NovaCategoria("A");
		await AdicionarAsync(categoria);

		var entidade = await Repositorio.ConsultarAsync(categoria.Id);
		entidade.ShouldNotBeNull();
		entidade.Nome = "Categoria Atualizada";
		entidade.Descricao = "Descricao atualizada";
		entidade.Ativo = false;

		await Repositorio.AtualizarAsync(entidade);
		await PersistirAsync();

		var salvo = await Contexto.Categorias.AsNoTracking().SingleAsync(c => c.Id == categoria.Id);

		salvo.Nome.ShouldBe("Categoria Atualizada");
		salvo.Descricao.ShouldBe("Descricao atualizada");
		salvo.Ativo.ShouldBeFalse();
	}

}
