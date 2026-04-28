using FIAP.FCG.Application.Categorias.Dtos;
using FIAP.FCG.Application.Categorias.Services;
using FIAP.FCG.Infrastructure.Dados;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FIAP.FCG.Web.API.Controllers;

[Tags("Categorias")]
public class CategoriasController : PadraoController
{
    private readonly ICategoriaService _service;
    private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;

    public CategoriasController(ICategoriaService service, IUnidadeDeTrabalho unidadeDeTrabalho)
    {
        _service = service;
        _unidadeDeTrabalho = unidadeDeTrabalho;
    }

    [AllowAnonymous]
    [HttpGet]
    [SwaggerOperation(Summary = "Listar categorias", Description = "Retorna todas as categorias com filtros opcionais por status e busca por nome.")]
    [ProducesResponseType(typeof(IEnumerable<CategoriaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] bool? ativo, [FromQuery] string? q)
    {
        var categorias = await _service.ListarAsync(ativo, q);
        return Ok(categorias);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obter categoria por ID", Description = "Retorna os detalhes de uma categoria específica.")]
    [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    [SwaggerOperation(Summary = "Adicionar categoria", Description = "Cria uma nova categoria. Requer perfil Administrador.")]
    [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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

    [Authorize(Roles = "Administrador")]
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar categoria", Description = "Atualiza os dados de uma categoria. Requer perfil Administrador.")]
    [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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

    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Remover categoria", Description = "Remove uma categoria pelo ID. Requer perfil Administrador.")]
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
}
