using FIAP.FCG.Application.ListaDeDesejos.Dtos;
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

        public async Task<IEnumerable<ListaDeDesejosDto>> ObterListaDeDesejosPorUsuarioAsync(int usuarioId)
        {
            var usuario = await _repository.ObterListaDeDesejosPorUsuarioAsync(usuarioId);

            if (usuario == null)
                throw new KeyNotFoundException("Usuário não encontrado");

            return usuario.Jogos.Select(j => new ListaDeDesejosDto
            {
                Id = j.Id,
                Nome = j.Nome,
                Descricao = j.Descricao,
                DataLancamento = j.DataLancamento
            });
        }

        public async Task AdicionarNovoJogoEmListaDeDesejos(int usuarioId, int jogoId)
        {
            await _repository.AdicionarNovoJogoEmListaDeDesejosAsync(usuarioId, jogoId);
            await _unidade.SalvarAsync();
        }

        
        public async Task RemoverJogoDaListaDeDesejos(int usuarioId, int jogoId)
        {
            await _repository.RemoverJogoDaListaDeDesejosAsync(usuarioId, jogoId);
            await _unidade.SalvarAsync();
        }
        
    }
}
