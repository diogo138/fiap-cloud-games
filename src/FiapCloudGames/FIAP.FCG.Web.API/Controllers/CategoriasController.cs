using FIAP.FCG.Application.Categorias.Dtos;
using FIAP.FCG.Application.Categorias.Services;
using FIAP.FCG.Infrastructure.Dados;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.FCG.Web.API.Controllers;

public class CategoriasController : PadraoController
{

	private readonly ICategoriaService _service;
	private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;

	public CategoriasController(ICategoriaService service, IUnidadeDeTrabalho unidadeDeTrabalho)
	{
		_service = service;
		_unidadeDeTrabalho = unidadeDeTrabalho;
	}

	[HttpGet]
	public async Task<IActionResult> Get([FromQuery] bool? ativo, [FromQuery] string? q)
	{
		var categorias = await _service.ListarAsync(ativo, q);
		return Ok(categorias);
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetById(int id)
	{
		try
		{
			var categoria = await _service.ConsultarAsync(id);
			return Ok(categoria);
		}
		catch (KeyNotFoundException ex)
		{
			return NotFound(new { erro = ex.Message });
		}
	}

	[HttpPost]
	public async Task<IActionResult> Post([FromBody] CategoriaNovoDto dto)
	{
		try
		{
			var categoria = await _service.AdicionarAsync(dto);
			await _unidadeDeTrabalho.SalvarAsync();
			return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, categoria);
		}
		catch (InvalidOperationException ex)
		{
			return Conflict(new { erro = ex.Message });
		}
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Put(int id, [FromBody] CategoriaAtualizadoDto dto)
	{
		try
		{
			var categoria = await _service.AtualizarAsync(id, dto);
			await _unidadeDeTrabalho.SalvarAsync();
			return Ok(categoria);
		}
		catch (KeyNotFoundException ex)
		{
			return NotFound(new { erro = ex.Message });
		}
		catch (InvalidOperationException ex)
		{
			return Conflict(new { erro = ex.Message });
		}
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(int id)
	{
		try
		{
			await _service.RemoverAsync(id);
			await _unidadeDeTrabalho.SalvarAsync();
			return NoContent();
		}
		catch (KeyNotFoundException ex)
		{
			return NotFound(new { erro = ex.Message });
		}
	}

}
