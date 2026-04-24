using FIAP.FCG.Application.ListaDeDesejos.Services;
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

        /* Métodos pré implementados para referência, podem ser ajustados conforme a necessidade do projeto
         
        [HttpGet("usuarios/{usuarioId}/lista-de-desejos")]
        public async Task<IActionResult> GetListaDeDesejos(int usuarioId)
        {
            var listaDeDesejos = await _service.ObterListaDeDesejosPorUsuarioAsync(usuarioId);
            return Ok(listaDeDesejos);
        }

        [HttpPost("usuarios/{usuarioId}/lista-de-desejos/{jogoId}")]
        public async Task<IActionResult> AdicionarJogoNaListaDeDesejos(int usuarioId, int jogoId)
        {
            await _service.AdicionarNovoJogoEmListaDeDesejos(usuarioId, jogoId);
            return NoContent();
        }

        [HttpDelete("usuarios/{usuarioId}/lista-de-desejos/{jogoId}")]
        public async Task<IActionResult> RemoverJogoDaListaDeDesejos(int usuarioId, int jogoId)
        {
            await _service.RemoverJogoDaListaDeDesejos(usuarioId, jogoId);
            return NoContent();
        }
        */
    }
}
