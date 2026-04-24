using FIAP.FCG.Application.Usuarios.Dtos;
using FIAP.FCG.Application.Usuarios.Services;
using FIAP.FCG.Infrastructure.Dados;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.FCG.Web.API.Controllers;

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
    public async Task<IActionResult> Get()
    {
        var usuarios = await _service.ListarAsync();
        return Ok(usuarios);
    }

    [HttpGet("{id}")]
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

    [HttpGet("{id}/biblioteca")]
    public async Task<IActionResult> GetBiblioteca(int id)
    {
        try
        {
            var biblioteca = await _service.ListarBibliotecaAsync(id);
            return Ok(biblioteca);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }

    [HttpGet("{usuarioId}/biblioteca/{jogoId}")]
    public async Task<IActionResult> GetBibliotecaDetalhada(int usuarioId, int jogoId)
    {
        try
        {
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
