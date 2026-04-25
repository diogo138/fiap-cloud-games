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

	[HttpGet]
	public async Task<IActionResult> GetCarrinhoUsuarioAtual()
	{
		var usuarioId = ContextoSeguranca.Usuario;
		var carrinhos = await _service.ListarPorUsuarioAsync(usuarioId);
		return Ok(carrinhos);
	}

	[HttpGet("{carrinhoId}")]
	public async Task<IActionResult> GetItemCarrinhoUsuarioAtual(int carrinhoId)
	{
		var usuarioId = ContextoSeguranca.Usuario;
		var carrinho = await _service.ListarPorUsuarioCarrinhoAsync(usuarioId, carrinhoId);
		return Ok(carrinho);
	}

	[HttpPost()]
	public async Task<IActionResult> Post([FromBody] CarrinhoNovoDto dto)
	{
		try
		{
			var usuarioId = ContextoSeguranca.Usuario;
			var carrinho = await _service.AdicionarAsync(usuarioId, dto);
			await _unidadeDeTrabalho.SalvarAsync();
			return CreatedAtAction(nameof(GetCarrinhoUsuarioAtual), new { usuarioId = carrinho.UsuarioId }, carrinho);
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(new { erro = ex.Message });
		}
	}

	[HttpPatch("{carrinhoId}")]
	public async Task<IActionResult> Patch(int carrinhoId, [FromBody] CarrinhoAtualizadoDto dto)
	{
		try
		{
			var usuarioId = ContextoSeguranca.Usuario;
			var carrinho = await _service.AtualizarQuantidadeAsync(usuarioId, carrinhoId, dto);
			await _unidadeDeTrabalho.SalvarAsync();
			return Ok(carrinho);
		}
		catch (KeyNotFoundException ex)
		{
			return BadRequest(new { erro = ex.Message });
		}
	}

	[HttpDelete("{carrinhoId}")]
	public async Task<IActionResult> Delete(int carrinhoId)
	{
		try
		{
			var usuarioId = ContextoSeguranca.Usuario;
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
