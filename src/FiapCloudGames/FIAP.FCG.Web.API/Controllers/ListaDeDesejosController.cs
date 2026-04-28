using FIAP.FCG.Application.ListaDeDesejos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FIAP.FCG.Web.API.Controllers
{

    [Tags("Lista de Desejos")]
    public class ListaDeDesejosController : PadraoController
    {
        private readonly IListaDeDesejosService _service;
        public ListaDeDesejosController(IListaDeDesejosService service)
        {
            _service = service;
        }

       
         
        [HttpGet("usuarios/{usuarioId}/lista-de-desejos")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Obter lista de desejos", Description = "Retorna a lista de desejos de um usuário.")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListaDeDesejos(int usuarioId)
        {
            var listaDeDesejos = await _service.ObterListaDeDesejosPorUsuarioAsync(usuarioId);
            return Ok(listaDeDesejos);
        }


        [HttpPost("usuarios/{usuarioId}/lista-de-desejos/{jogoId}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Adicionar jogo à lista de desejos", Description = "Adiciona um jogo à lista de desejos do usuário.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [SwaggerOperation(Summary = "Remover jogo da lista de desejos", Description = "Remove um jogo da lista de desejos do usuário.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
