using FIAP.FCG.Infrastructure.Dados;
using FIAP.FCG.Infrastructure.Dados.Repositorios;

namespace FIAP.FCG.Application.ListaDeDesejos.Services
{
    public class ListaDeDesejosService : IListaDeDesejosService
    {
        private readonly IListaDeDesejosRepository _repository;
        private readonly IUnidadeDeTrabalho _unidade;

        public ListaDeDesejosService(IListaDeDesejosRepository repository, IUnidadeDeTrabalho unidade)
        {
            _repository = repository;
            _unidade = unidade;
        }

        /* Métodos pré implementados para referência, podem ser ajustados conforme a necessidade do projeto
         
        public async Task<ListaDeDesejosDto> ObterListaDeDesejosPorUsuarioAsync(int usuarioId)
        {
            var listaDeDesejos = await _repository.ObterListaDeDesejosPorUsuarioAsync(usuarioId); 
            return listaDeDesejos;
        }


        public async Task AdicionarNovoJogoEmListaDeDesejos(int usuarioId, int jogoId)
        {
            await _repository.AdicionarNovoJogoEmListaDeDesejos(usuarioId, jogoId);
            await _unidade.SalvarAlteracoesAsync();
        }

        public async Task RemoverJogoDaListaDeDesejos(int usuarioId, int jogoId)
        {
            await _repository.RemoverJogoDaListaDeDesejos(usuarioId, jogoId);
            await _unidade.SalvarAlteracoesAsync();
        }
        */
    }
}
