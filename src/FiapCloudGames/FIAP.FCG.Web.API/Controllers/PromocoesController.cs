using FIAP.FCG.Application.Promocoes.Dtos;
using FIAP.FCG.Application.Promocoes.Services;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.FCG.Web.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PromocoesController : ControllerBase
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
            if (promocaoNova == null)
            {
                return BadRequest();
            }
            var novaoPromocao = await _service.AdicionarPromocaoAsync(promocaoNova);
            return Ok(novaoPromocao);
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

        /*
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePromocao(int id, [FromBody] UpPromocaoReq upPromocaoReq)
        {
            var promocao = await _context.Promocoes.FindAsync(id);

            if (promocao == null)
            {
                return NotFound();
            }

            Promocao updatedPromocao = new Promocao
            {
                Id = id,
                Nome = upPromocaoReq.Nome,
                DataCadastro = upPromocaoReq.DataCadastro,
                DataInicio = upPromocaoReq.DataInicio,
                DataFim = upPromocaoReq.DataFim,
                PercentualDesconto = upPromocaoReq.PercentualDesconto
            };

            if (updatedPromocao.Equals(promocao))
            {
                return BadRequest();
            }

            promocao.Nome = upPromocaoReq.Nome;
            promocao.DataCadastro = upPromocaoReq.DataCadastro;
            promocao.DataInicio = upPromocaoReq.DataInicio;
            promocao.DataFim = upPromocaoReq.DataFim;
            promocao.PercentualDesconto = upPromocaoReq.PercentualDesconto;

            await _context.SaveChangesAsync();
            return BadRequest();
        }
        */
    }
}
