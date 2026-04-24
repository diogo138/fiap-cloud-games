using FIAP.FCG.Application.Promocoes.Dtos;
using FIAP.FCG.Application.Promocoes.Services;
using FIAP.FCG.Infrastructure.Dados.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FIAP.FCG.Web.API.Controllers
{

    public class PromocoesController : PadraoController
    {
        private readonly IPromocoesService _service;

        public PromocoesController(IPromocoesService service)
        {
            _service = service;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var promocoes = await _service.ListarPromocoesAsync();
            return Ok(promocoes);
        }

        [HttpGet("{promocaoId}")]
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
        public async Task<IActionResult> Delete(int promocaoId)
        {
            await _service.RemoverPromocaoAsync(promocaoId);
            return Ok();
        }

        [HttpPost("{promocaoId}/jogos/{jogoId}")]
        public async Task<IActionResult> IncluirJogoNaPromocao(int promocaoId, int jogoId)
        {
            var novoJogoEmPromocao = await _service.AdicionarNovoJogoEmPromocaoAsync(promocaoId, jogoId);
            return Ok(novoJogoEmPromocao);
        }

        [HttpDelete("{promocaoId}/jogos/{jogoId}")]
        public async Task<IActionResult> RemoverJogoEmPromocao(int promocaoId, int jogoId)
        {
            await _service.RemoverNovoJogoEmPromocaoAsync(promocaoId, jogoId);
            return Ok();
        }
        
        
        [HttpPut("{promocaoId}")]
        public async Task<IActionResult> UpdatePromocao(int promocaoId, [FromBody] PromocaoAtualizarDto promocaoAtualizarDto)
        {
            var promocao = await _service.AtualizarPromocaoAsync(promocaoId, promocaoAtualizarDto);
            return Ok(promocao);
        }
    }
}
