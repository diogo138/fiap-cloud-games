using FIAP.FCG.Application.Usuarios.Dtos;
using FIAP.FCG.Application.Usuarios.Services;
using FIAP.FCG.Infrastructure.Dados;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FIAP.FCG.Web.API.Controllers;

[Tags("Usuários")]
public class UsuariosController : PadraoController
{
	private readonly IUsuarioService _service;
	private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;

	public UsuariosController(IUsuarioService service, IUnidadeDeTrabalho unidadeDeTrabalho)
	{
		_service = service;
		_unidadeDeTrabalho = unidadeDeTrabalho;
	}

	[Authorize(Roles = "Administrador")]
	[HttpGet]
	[SwaggerOperation(Summary = "Listar usuários", Description = "Retorna todos os usuários cadastrados. Requer perfil Administrador.")]
	[ProducesResponseType(typeof(IEnumerable<UsuarioDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> Get()
	{
		var usuarios = await _service.ListarAsync();
		return Ok(usuarios);
	}

	[HttpGet("{id}")]
	[SwaggerOperation(Summary = "Obter usuário por ID", Description = "Retorna os detalhes de um usuário específico.")]
	[ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetById(int id)
	{
		try
		{
			var usuario = await _service.ConsultarAsync(id);
			return Ok(usuario);
		}
		catch (KeyNotFoundException ex)
		{
			return NotFound(new { erro = ex.Message });
		}
	}

	[AllowAnonymous]
	[HttpPost]
	[SwaggerOperation(Summary = "Registrar usuário", Description = "Cria uma nova conta de usuário.")]
	[ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status409Conflict)]
	public async Task<IActionResult> Post([FromBody] UsuarioNovoDto dto)
	{
		try
		{
			var usuario = await _service.AdicionarAsync(dto);
			await _unidadeDeTrabalho.SalvarAsync();
			return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
		}
		catch (InvalidOperationException ex)
		{
			return Conflict(new { erro = ex.Message });
		}
	}

	[Authorize(Roles = "Administrador")]
	[HttpPut("{id}")]
	[SwaggerOperation(Summary = "Atualizar usuário", Description = "Atualiza os dados de um usuário. Requer perfil Administrador.")]
	[ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Put(int id, [FromBody] UsuarioAtualizadoDto dto)
	{
		try
		{
			var usuario = await _service.AtualizarAsync(id, dto);
			await _unidadeDeTrabalho.SalvarAsync();
			return Ok(usuario);
		}
		catch (KeyNotFoundException ex)
		{
			return NotFound(new { erro = ex.Message });
		}
	}

	[Authorize(Roles = "Administrador")]
	[HttpDelete("{id}")]
	[SwaggerOperation(Summary = "Remover usuário", Description = "Remove um usuário pelo ID. Requer perfil Administrador.")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
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

	[Authorize(Roles = "Administrador")]
	[HttpPut("{id}/admin")]
	[SwaggerOperation(Summary = "Conceder perfil Administrador", Description = "Eleva um usuário ao perfil Administrador. Requer perfil Administrador.")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> ConcederAdmin(int id)
	{
		try
		{
			await _service.ConcederAdminAsync(id);
			await _unidadeDeTrabalho.SalvarAsync();
			return NoContent();
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

	[Authorize(Roles = "Administrador")]
	[HttpDelete("{id}/admin")]
	[SwaggerOperation(Summary = "Revogar perfil Administrador", Description = "Remove o perfil de Administrador de um usuário. Requer perfil Administrador.")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> RevogarAdmin(int id)
	{
		try
		{
			await _service.RevogarAdminAsync(id);
			await _unidadeDeTrabalho.SalvarAsync();
			return NoContent();
		}
		catch (KeyNotFoundException ex)
		{
			return NotFound(new { erro = ex.Message });
		}
	}

	[HttpGet("biblioteca")]
	[SwaggerOperation(Summary = "Listar biblioteca do usuário", Description = "Retorna os jogos comprados pelo usuário autenticado.")]
	[ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetBiblioteca()
	{
		try
		{
			var usuarioId = ContextoSeguranca.Usuario;
			var biblioteca = await _service.ListarBibliotecaAsync(usuarioId);
			return Ok(biblioteca);
		}
		catch (KeyNotFoundException ex)
		{
			return NotFound(new { erro = ex.Message });
		}
	}

	[HttpGet("biblioteca/{jogoId}")]
	public async Task<IActionResult> GetBibliotecaDetalhada(int jogoId)
	{
		try
		{
			var usuarioId = ContextoSeguranca.Usuario;
			var biblioteca = await _service.ListarBibliotecaAsync(usuarioId, jogoId);
			if (biblioteca == null)
				return NoContent();
			return Ok(biblioteca);
		}
		catch (KeyNotFoundException ex)
		{
			return NotFound(new { erro = ex.Message });
		}
	}
}
