using FIAP.FCG.Application.Carrinhos.Dtos;
using FIAP.FCG.Application.Carrinhos.Services;
using FIAP.FCG.Infrastructure.Dados;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.FCG.Web.API.Controllers;

public class CarrinhosController : PadraoController
{

	private readonly ICarrinhoService _service;
	private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;

	public CarrinhosController(ICarrinhoService service, IUnidadeDeTrabalho unidadeDeTrabalho)
	{
		_service = service;
		_unidadeDeTrabalho = unidadeDeTrabalho;
	}

	[HttpGet("{usuarioId}")]
	public async Task<IActionResult> GetPorUsuario(int usuarioId)
	{
		var carrinhos = await _service.ListarPorUsuarioAsync(usuarioId);
		return Ok(carrinhos);
	}

	[HttpGet("{usuarioId}/{carrinhoId}")]
	public async Task<IActionResult> GetPorUsuarioCarrinho(int usuarioId, int carrinhoId)
	{
		var carrinho = await _service.ListarPorUsuarioCarrinhoAsync(usuarioId, carrinhoId);
		return Ok(carrinho);
	}

	[HttpPost("{id}")]
	public async Task<IActionResult> Post(int id, [FromBody] CarrinhoNovoDto dto)
	{
		try
		{
			var carrinho = await _service.AdicionarAsync(id, dto);
			await _unidadeDeTrabalho.SalvarAsync();
			return CreatedAtAction(nameof(GetPorUsuario), new { usuarioId = carrinho.UsuarioId }, carrinho);
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(new { erro = ex.Message });
		}
	}

	[HttpPatch("{usuarioId}/{carrinhoId}")]
	public async Task<IActionResult> Patch(int usuarioId, int carrinhoId, [FromBody] CarrinhoAtualizadoDto dto)
	{
		try
		{
			var carrinho = await _service.AtualizarQuantidadeAsync(usuarioId, carrinhoId, dto);
			await _unidadeDeTrabalho.SalvarAsync();
			return Ok(carrinho);
		}
		catch (KeyNotFoundException ex)
		{
			return BadRequest(new { erro = ex.Message });
		}
	}

	[HttpDelete("{usuarioId}/{carrinhoId}")]
	public async Task<IActionResult> Delete(int usuarioId, int carrinhoId)
	{
		try
		{
			await _service.RemoverAsync(usuarioId, carrinhoId);
			await _unidadeDeTrabalho.SalvarAsync();
			return Ok();
		}
		catch (KeyNotFoundException ex)
		{
			return BadRequest(new { erro = ex.Message });
		}
	}

}
