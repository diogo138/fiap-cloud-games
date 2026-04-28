using FIAP.FCG.Application.Jogos.Dtos;
using FIAP.FCG.Application.Jogos.Services;
using FIAP.FCG.Infrastructure.Dados;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FIAP.FCG.Web.API.Controllers;

[Tags("Jogos")]
public class JogosController : PadraoController
{
    private readonly IJogoService _service;
    private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;

    public JogosController(IJogoService service, IUnidadeDeTrabalho unidadeDeTrabalho)
    {
        _service = service;
        _unidadeDeTrabalho = unidadeDeTrabalho;
    }

    [AllowAnonymous]
    [HttpGet]
    [SwaggerOperation(Summary = "Listar jogos", Description = "Retorna a lista de jogos com filtros opcionais por categoria, visibilidade, status e busca por nome.")]
    [ProducesResponseType(typeof(IEnumerable<JogoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(
        [FromQuery] int? categoriaId,
        [FromQuery] bool? visivel,
        [FromQuery] bool? ativo,
        [FromQuery] string? q)
    {
        var jogos = await _service.ListarAsync(categoriaId, visivel, ativo, q);
        return Ok(jogos);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obter jogo por ID", Description = "Retorna os detalhes de um jogo específico.")]
    [ProducesResponseType(typeof(JogoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    [SwaggerOperation(Summary = "Adicionar jogo", Description = "Cria um novo jogo. Requer perfil Administrador.")]
    [ProducesResponseType(typeof(JogoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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

    [Authorize(Roles = "Administrador")]
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar jogo", Description = "Atualiza os dados de um jogo existente. Requer perfil Administrador.")]
    [ProducesResponseType(typeof(JogoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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

    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Remover jogo", Description = "Remove um jogo pelo ID. Requer perfil Administrador.")]
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
    [HttpPost("{id}/ajustar-preco")]
    [SwaggerOperation(Summary = "Ajustar preço do jogo", Description = "Aplica um novo preço a um jogo. Requer perfil Administrador.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
