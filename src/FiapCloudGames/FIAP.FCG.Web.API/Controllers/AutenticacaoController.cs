using FIAP.FCG.Application.Autenticacao.Dtos;
using FIAP.FCG.Application.Autenticacao.Services;
using FIAP.FCG.Infrastructure.Dados;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.FCG.Web.API.Controllers;

[AllowAnonymous]
public class AutenticacaoController : PadraoController
{
	private readonly IAutenticacaoService _service;
	private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;

	public AutenticacaoController(IAutenticacaoService service, IUnidadeDeTrabalho unidadeDeTrabalho)
	{
		_service = service;
		_unidadeDeTrabalho = unidadeDeTrabalho;
	}

	[HttpPost]
	public async Task<IActionResult> Post([FromBody] AutenticacaoDto dto)
	{
		try
		{
			var token = await _service.AutenticarAsync(dto);
			await _unidadeDeTrabalho.SalvarAsync();
			return Ok(token);
		}
		catch (UnauthorizedAccessException ex)
		{
			return Unauthorized(new { erro = ex.Message });
		}
	}
}
