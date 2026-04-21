using FIAP.FCG.Application.Jogos.Dtos;
using FIAP.FCG.Application.Jogos.Services;
using FIAP.FCG.Infrastructure.Dados;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.FCG.Web.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogosController : ControllerBase
{

	private readonly IJogoService _service;
	private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;

	public JogosController(IJogoService service, IUnidadeDeTrabalho unidadeDeTrabalho)
	{
		_service = service;
		_unidadeDeTrabalho = unidadeDeTrabalho;
	}

	[HttpGet]
	public async Task<IActionResult> Get(
		[FromQuery] int? categoriaId,
		[FromQuery] bool? visivel,
		[FromQuery] bool? ativo,
		[FromQuery] string? q)
	{
		var jogos = await _service.ListarAsync(categoriaId, visivel, ativo, q);
		return Ok(jogos);
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetById(int id)
	{
		try
		{
			var jogo = await _service.ConsultarAsync(id);
			return Ok(jogo);
		}
		catch (KeyNotFoundException ex)
		{
			return NotFound(new { erro = ex.Message });
		}
	}

	[HttpPost]
	public async Task<IActionResult> Post([FromBody] JogoNovoDto dto)
	{
		try
		{
			var jogo = await _service.AdicionarAsync(dto);
			await _unidadeDeTrabalho.SalvarAsync();
			return CreatedAtAction(nameof(GetById), new { id = jogo.Id }, jogo);
		}
		catch (InvalidOperationException ex)
		{
			return Conflict(new { erro = ex.Message });
		}
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Put(int id, [FromBody] JogoAtualizadoDto dto)
	{
		try
		{
			var jogo = await _service.AtualizarAsync(id, dto);
			await _unidadeDeTrabalho.SalvarAsync();
			return Ok(jogo);
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

	[HttpPost("{id}/ajustar-preco")]
	public async Task<IActionResult> AjustarPreco(int id, [FromBody] AjustarPrecoDto dto)
	{
		try
		{
			await _service.AjustarPrecoAsync(id, dto);
			await _unidadeDeTrabalho.SalvarAsync();
			return NoContent();
		}
		catch (KeyNotFoundException ex)
		{
			return NotFound(new { erro = ex.Message });
		}
	}

}
