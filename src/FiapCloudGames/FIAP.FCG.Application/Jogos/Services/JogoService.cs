using FIAP.FCG.Application.Jogos.Dtos;
using FIAP.FCG.Domain.Jogos;
using FIAP.FCG.Infrastructure.Dados.Entidades;
using FIAP.FCG.Infrastructure.Dados.Repositorios;

namespace FIAP.FCG.Application.Jogos.Services;

public class JogoService : IJogoService
{

	private readonly IJogoRepository _repositorio;
	private readonly IPrecoJogoRepository _precoJogoRepositorio;
	private readonly IJogoBusiness _business;

	public JogoService(IJogoRepository repositorio, IPrecoJogoRepository precoJogoRepositorio, IJogoBusiness business)
	{
		_repositorio = repositorio;
		_precoJogoRepositorio = precoJogoRepositorio;
		_business = business;
	}

	public async Task<IEnumerable<JogoDto>> ListarAsync(int? categoriaId, bool? visivel, bool? ativo, string? q)
	{
		var jogos = await _repositorio.ConsultarTodosAsync(categoriaId, visivel, ativo, q);
		return jogos.Select(MapearJogo);
	}

	public async Task<JogoDto> ConsultarAsync(int id)
	{
		var jogo = await _repositorio.ConsultarAsync(id);

		if (jogo == null)
			throw new KeyNotFoundException("Jogo não encontrado.");

		return MapearJogo(jogo);
	}

	public async Task<JogoDto> AdicionarAsync(JogoNovoDto dto)
	{
		if (await _business.NomeJaExiste(dto.Nome))
			throw new InvalidOperationException("Já existe um jogo com este nome.");

		var jogo = new Jogo
		{
			Nome = dto.Nome,
			CategoriaId = dto.CategoriaId,
			Descricao = dto.Descricao,
			DataLancamento = dto.DataLancamento,
			Visivel = dto.Visivel,
			DataCadastro = DateTime.UtcNow,
			Ativo = true
		};

		await _repositorio.AdicionarAsync(jogo);

		return MapearJogo(jogo);
	}

	public async Task<JogoDto> AtualizarAsync(int id, JogoAtualizadoDto dto)
	{
		var jogo = await _repositorio.ConsultarAsync(id);

		if (jogo == null)
			throw new KeyNotFoundException("Jogo não encontrado.");

		if (await _business.NomeJaExiste(dto.Nome, excluirId: id))
			throw new InvalidOperationException("Já existe um jogo com este nome.");

		jogo.Nome = dto.Nome;
		jogo.CategoriaId = dto.CategoriaId;
		jogo.Descricao = dto.Descricao;
		jogo.DataLancamento = dto.DataLancamento;
		jogo.Visivel = dto.Visivel;
		jogo.Ativo = dto.Ativo;

		await _repositorio.AtualizarAsync(jogo);

		return MapearJogo(jogo);
	}

	public async Task RemoverAsync(int id)
	{
		var jogo = await _repositorio.ConsultarAsync(id);

		if (jogo == null)
			throw new KeyNotFoundException("Jogo não encontrado.");

		jogo.Ativo = false;

		await _repositorio.AtualizarAsync(jogo);
	}

	public async Task AjustarPrecoAsync(int id, AjustarPrecoDto dto)
	{
		var jogo = await _repositorio.ConsultarAsync(id);

		if (jogo == null)
			throw new KeyNotFoundException("Jogo não encontrado.");

		var precoJogo = new PrecoJogo
		{
			JogoId = id,
			Valor = dto.Valor,
			PromocaoId = dto.PromocaoId,
			PercDesconto = dto.PercDesconto,
			DataInicio = DateTime.UtcNow
		};

		await _precoJogoRepositorio.AdicionarAsync(precoJogo);
	}

	private static JogoDto MapearJogo(Jogo jogo) => new()
	{
		Id = jogo.Id,
		Nome = jogo.Nome,
		CategoriaId = jogo.CategoriaId,
		Descricao = jogo.Descricao,
		DataCadastro = jogo.DataCadastro,
		DataLancamento = jogo.DataLancamento,
		Visivel = jogo.Visivel,
		Ativo = jogo.Ativo
	};

}
