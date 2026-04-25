using FIAP.FCG.Application.ListaDeDesejos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.FCG.Web.API.Controllers
{

    public class ListaDeDesejosController : PadraoController
    {
        private readonly IListaDeDesejosService _service;
        public ListaDeDesejosController(IListaDeDesejosService service)
        {
            _service = service;
        }

       
         
        [HttpGet("usuarios/{usuarioId}/lista-de-desejos")]
        [AllowAnonymous]
        public async Task<IActionResult> GetListaDeDesejos(int usuarioId)
        {
            var listaDeDesejos = await _service.ObterListaDeDesejosPorUsuarioAsync(usuarioId);
            return Ok(listaDeDesejos);
        }


        [HttpPost("usuarios/{usuarioId}/lista-de-desejos/{jogoId}")]
        [AllowAnonymous]
        public async Task<IActionResult> AdicionarJogoNaListaDeDesejos(int usuarioId, int jogoId)
        {
            try
            {
                await _service.AdicionarNovoJogoEmListaDeDesejos(usuarioId, jogoId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { erro = ex.Message });
            }
        }


        [HttpDelete("usuarios/{usuarioId}/lista-de-desejos/{jogoId}")]
        [AllowAnonymous]
        public async Task<IActionResult> RemoverJogoDaListaDeDesejos(int usuarioId, int jogoId)
        {
            try
            {
                await _service.RemoverJogoDaListaDeDesejos(usuarioId, jogoId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { erro = ex.Message });
            }
        }

    }
}
