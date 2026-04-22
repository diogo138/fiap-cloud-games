using FIAP.FCG.Domain.Jogos;
using FIAP.FCG.Tests;
using Shouldly;

namespace FIAP.FCG.Domain.Tests.Jogos;

public class JogoBusinessTest : PadraoTest
{

	private IJogoBusiness Business => CarregarInstancia<IJogoBusiness>();

	[Test]
	public async Task NomeJaExiste_QuandoNomeEstiverCadastrado_DeveRetornarTrue()
	{
		var categoria = DataFakeFactory.NovaCategoria();
		var jogo = DataFakeFactory.NovoJogo("A", categoria);
		await AdicionarAsync(categoria, jogo);

		var resultado = await Business.NomeJaExiste(jogo.Nome);

		resultado.ShouldBeTrue();
	}

	[Test]
	public async Task NomeJaExiste_QuandoNomeNaoEstiverCadastrado_DeveRetornarFalse()
	{
		var resultado = await Business.NomeJaExiste("Jogo Inexistente");

		resultado.ShouldBeFalse();
	}

	[Test]
	public async Task NomeJaExiste_QuandoNomeForDoProprioRegistro_DeveRetornarFalse()
	{
		var categoria = DataFakeFactory.NovaCategoria();
		var jogo = DataFakeFactory.NovoJogo("A", categoria);
		await AdicionarAsync(categoria, jogo);

		var resultado = await Business.NomeJaExiste(jogo.Nome, excluirId: jogo.Id);

		resultado.ShouldBeFalse();
	}

}
