using FIAP.FCG.Domain.Categorias;
using FIAP.FCG.Tests;
using Shouldly;

namespace FIAP.FCG.Domain.Tests.Categorias;

public class CategoriaBusinessTest : PadraoTest
{

	private ICategoriaBusiness Business => CarregarInstancia<ICategoriaBusiness>();

	[Test]
	public async Task NomeJaExiste_QuandoNomeEstiverCadastrado_DeveRetornarTrue()
	{
		var categoria = DataFakeFactory.NovaCategoria();
		await AdicionarAsync(categoria);

		var resultado = await Business.NomeJaExiste(categoria.Nome);

		resultado.ShouldBeTrue();
	}

	[Test]
	public async Task NomeJaExiste_QuandoNomeNaoEstiverCadastrado_DeveRetornarFalse()
	{
		var resultado = await Business.NomeJaExiste("Categoria Inexistente");

		resultado.ShouldBeFalse();
	}

	[Test]
	public async Task NomeJaExiste_QuandoNomeForDoProprioRegistro_DeveRetornarFalse()
	{
		var categoria = DataFakeFactory.NovaCategoria();
		await AdicionarAsync(categoria);

		var resultado = await Business.NomeJaExiste(categoria.Nome, excluirId: categoria.Id);

		resultado.ShouldBeFalse();
	}

}
