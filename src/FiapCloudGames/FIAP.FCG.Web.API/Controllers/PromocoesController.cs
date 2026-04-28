using FIAP.FCG.Application.Promocoes.Dtos;
using FIAP.FCG.Application.Promocoes.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FIAP.FCG.Web.API.Controllers
{

    [Tags("Promoções")]
    public class PromocoesController : PadraoController
    {
        private readonly IPromocoesService _service;

        public PromocoesController(IPromocoesService service)
        {
            _service = service;
        }


        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Listar promoções", Description = "Retorna a lista de promoções com filtro por nome, paginação e ordenação.")]
        [ProducesResponseType(typeof(IEnumerable<PromocaoDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] string? nome, [FromQuery] int page = 1, [FromQuery] string? orderBy = "dataCadastro", [FromQuery] bool desc = false)
        {
            var promocoes = await _service.ListarPromocoesAsync(nome, page, orderBy, desc);
            return Ok(promocoes);
        }


        [HttpGet("{promocaoId}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Obter promoção por ID", Description = "Retorna os detalhes de uma promoção específica.")]
        [ProducesResponseType(typeof(PromocaoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int promocaoId)
        {
            var promocao = await _service.ObterPromocaoPorIdAsync(promocaoId);
            if (promocao == null)
            {
                return NotFound();
            }
            return Ok(promocao);
        }


        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [SwaggerOperation(Summary = "Criar promoção", Description = "Cria uma nova promoção. Requer perfil Administrador.")]
        [ProducesResponseType(typeof(PromocaoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] PromocaoNovaDto promocaoNova)
        {
            try
            {
                if (promocaoNova == null)
                {
                    return BadRequest();
                }
                var novaoPromocao = await _service.AdicionarPromocaoAsync(promocaoNova);
                return Ok(novaoPromocao);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { erro = ex.Message });
            }

        }


        [HttpDelete("{promocaoId}")]
        [Authorize(Roles = "Administrador")]
        [SwaggerOperation(Summary = "Remover promoção", Description = "Remove uma promoção pelo ID. Requer perfil Administrador.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(int promocaoId)
        {
            await _service.RemoverPromocaoAsync(promocaoId);
            return Ok();
        }


        [HttpPost("{promocaoId}/jogos/{jogoId}")]
        [Authorize(Roles = "Administrador")]
        [SwaggerOperation(Summary = "Incluir jogo na promoção", Description = "Associa um jogo a uma promoção. Requer perfil Administrador.")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> IncluirJogoNaPromocao(int promocaoId, int jogoId)
        {
            var novoJogoEmPromocao = await _service.AdicionarNovoJogoEmPromocaoAsync(promocaoId, jogoId);
            return Ok(novoJogoEmPromocao);
        }


        [HttpDelete("{promocaoId}/jogos/{jogoId}")]
        [Authorize(Roles = "Administrador")]
        [SwaggerOperation(Summary = "Remover jogo da promoção", Description = "Remove a associação de um jogo com uma promoção. Requer perfil Administrador.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RemoverJogoEmPromocao(int promocaoId, int jogoId)
        {
            await _service.RemoverNovoJogoEmPromocaoAsync(promocaoId, jogoId);
            return Ok();
        }


        [HttpPut("{promocaoId}")]
        [Authorize(Roles = "Administrador")]
        [SwaggerOperation(Summary = "Atualizar promoção", Description = "Atualiza os dados de uma promoção. Requer perfil Administrador.")]
        [ProducesResponseType(typeof(PromocaoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdatePromocao(int promocaoId, [FromBody] PromocaoAtualizarDto promocaoAtualizarDto)
        {
            var promocao = await _service.AtualizarPromocaoAsync(promocaoId, promocaoAtualizarDto);
            return Ok(promocao);
        }
    }
}
