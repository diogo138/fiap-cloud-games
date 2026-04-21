using FIAP.FCG.Application.Categorias.Dtos;
using FIAP.FCG.Domain.Categorias;
using FIAP.FCG.Infrastructure.Dados.Entidades;
using FIAP.FCG.Infrastructure.Dados.Repositorios;

namespace FIAP.FCG.Application.Categorias.Services;

public class CategoriaService : ICategoriaService
{

	private readonly ICategoriaRepository _repositorio;
	private readonly ICategoriaBusiness _business;

	public CategoriaService(ICategoriaRepository repositorio, ICategoriaBusiness business)
	{
		_repositorio = repositorio;
		_business = business;
	}

	public async Task<IEnumerable<CategoriaDto>> ListarAsync(bool? ativo, string? q)
	{
		var categorias = await _repositorio.ConsultarTodosAsync(ativo, q);
		return categorias.Select(MapearCategoria);
	}

	public async Task<CategoriaDto> ConsultarAsync(int id)
	{
		var categoria = await _repositorio.ConsultarAsync(id);

		if (categoria == null)
			throw new KeyNotFoundException("Categoria não encontrada.");

		return MapearCategoria(categoria);
	}

	public async Task<CategoriaDto> AdicionarAsync(CategoriaNovoDto dto)
	{
		if (await _business.NomeJaExiste(dto.Nome))
			throw new InvalidOperationException("Já existe uma categoria com este nome.");

		var categoria = new Categoria
		{
			Nome = dto.Nome,
			Descricao = dto.Descricao,
			DataCadastro = DateTime.UtcNow,
			Ativo = true
		};

		await _repositorio.AdicionarAsync(categoria);

		return MapearCategoria(categoria);
	}

	public async Task<CategoriaDto> AtualizarAsync(int id, CategoriaAtualizadoDto dto)
	{
		var categoria = await _repositorio.ConsultarAsync(id);

		if (categoria == null)
			throw new KeyNotFoundException("Categoria não encontrada.");

		if (await _business.NomeJaExiste(dto.Nome, excluirId: id))
			throw new InvalidOperationException("Já existe uma categoria com este nome.");

		categoria.Nome = dto.Nome;
		categoria.Descricao = dto.Descricao;
		categoria.Ativo = dto.Ativo;

		await _repositorio.AtualizarAsync(categoria);

		return MapearCategoria(categoria);
	}

	public async Task RemoverAsync(int id)
	{
		var categoria = await _repositorio.ConsultarAsync(id);

		if (categoria == null)
			throw new KeyNotFoundException("Categoria não encontrada.");

		categoria.Ativo = false;

		await _repositorio.AtualizarAsync(categoria);
	}

	private static CategoriaDto MapearCategoria(Categoria categoria) => new()
	{
		Id = categoria.Id,
		Nome = categoria.Nome,
		Descricao = categoria.Descricao,
		DataCadastro = categoria.DataCadastro,
		Ativo = categoria.Ativo
	};

}
