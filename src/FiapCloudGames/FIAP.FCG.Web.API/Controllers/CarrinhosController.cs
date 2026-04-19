using FIAP.FCG.Application.Carrinhos.Dtos;
using FIAP.FCG.Application.Carrinhos.Services;
using FIAP.FCG.Infrastructure.Dados;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.FCG.Web.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarrinhosController : ControllerBase
{

	private readonly ICarrinhoService _service;
	private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;

	public CarrinhosController(ICarrinhoService service, IUnidadeDeTrabalho unidadeDeTrabalho)
	{
		_service = service;
		_unidadeDeTrabalho = unidadeDeTrabalho;
	}

	[HttpGet("usuario/{usuarioId}")]
	public async Task<IActionResult> GetByUsuario(int usuarioId)
	{
		var carrinhos = await _service.ListarPorUsuarioAsync(usuarioId);
		return Ok(carrinhos);
	}

	[HttpPost]
	public async Task<IActionResult> Post([FromBody] CarrinhoNovoDto dto)
	{
		try
		{
			var carrinho = await _service.AdicionarAsync(dto);
			await _unidadeDeTrabalho.SalvarAsync();
			return CreatedAtAction(nameof(GetByUsuario), new { usuarioId = carrinho.UsuarioId }, carrinho);
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(new { erro = ex.Message });
		}
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Put(int id, [FromBody] CarrinhoAtualizadoDto dto)
	{
		try
		{
			var carrinho = await _service.AtualizarQuantidadeAsync(id, dto);
			await _unidadeDeTrabalho.SalvarAsync();
			return Ok(carrinho);
		}
		catch (KeyNotFoundException ex)
		{
			return NotFound(new { erro = ex.Message });
		}
	}

}
