using FIAP.FCG.Application.Promocoes.Dtos;
using FIAP.FCG.Infrastructure.Dados;
using FIAP.FCG.Infrastructure.Dados.Entidades;
using FIAP.FCG.Infrastructure.Dados.Repositorios;

namespace FIAP.FCG.Application.Promocoes.Services
{
    public class PromocoesService : IPromocoesService
    {
        private readonly IPromocoesRespository _repositorio;
        private readonly IPrecoJogoRepository _jogoRepository;
        private readonly IUnidadeDeTrabalho _unidade;

        public PromocoesService(IPromocoesRespository repositorio, IUnidadeDeTrabalho unidade, IPrecoJogoRepository jogoRepository)
        {
            _repositorio = repositorio;
            _unidade = unidade;
            _jogoRepository = jogoRepository;
        }

        public async Task<IEnumerable<PromocaoDto>> ListarPromocoesAsync()
        {
            var promocoes = await _repositorio.ConsultarPromocoesAsync();
            return promocoes.Select(MapearPromocao);
        }

        public async Task<PromocaoDto> ObterPromocaoPorIdAsync(int promocaoId)
        {
            var promocao = await _repositorio.ObterPromocaoPorIdAsync(promocaoId);
            return MapearPromocao(promocao);
        }

        public async Task<PromocaoDto> AdicionarPromocaoAsync(PromocaoNovaDto promocaoNovaDto)
        {
            var promocao = MapearNovaPromocao(promocaoNovaDto);
            await _repositorio.AdicionarPromocaoAsync(promocao);
            await _unidade.SalvarAsync();
            return MapearPromocao(promocao);
        }

        public async Task RemoverPromocaoAsync(int promocaoId)
        {
            await _repositorio.RemoverPromocaoAsync(promocaoId);
            await _unidade.SalvarAsync();
        }

        public async Task<PromocaoNovoJogoDto> AdicionarNovoJogoEmPromocaoAsync(int promocaoId, int jogoId)
        {
            var promocao = await ObterPromocaoPorIdAsync(promocaoId);
            var valorMaisRecente = await _jogoRepository.ObterHistoricoDePrecoAsync(jogoId);

            var novoJogoEmPromocao = new PrecoJogo
            {
                JogoId = jogoId,
                Valor = valorMaisRecente,
                PromocaoId = promocaoId,
                PercDesconto = promocao.PercDesconto,
                DataInicio = promocao.DataInicio,
            };

            await _jogoRepository.AdicionarAsync(novoJogoEmPromocao);
            await _unidade.SalvarAsync();

            return MapearPromocaoJogo(novoJogoEmPromocao);
        }

        public async Task RemoverNovoJogoEmPromocaoAsync(int promocaoId, int jogoId)
        {
            await _jogoRepository.RemoverAsync(promocaoId, jogoId);
            await _unidade.SalvarAsync();
        }


        // ----------------------------------------- Métodos Auxiliares -----------------------------------------
        private static Promocao MapearNovaPromocao(PromocaoNovaDto promocaoNova)
        {
            return new Promocao
            {
                Nome = promocaoNova.Nome,
                DataCadastro = DateTime.UtcNow,
                DataInicio = promocaoNova.DataInicio,
                DataFim = promocaoNova.DataFim,
                PercDesconto = promocaoNova.PercDesconto
            };
        }

        private static PromocaoDto MapearPromocao(Promocao promocao)
        {
            return new PromocaoDto
            {
                Id = promocao.Id,
                Nome = promocao.Nome,
                DataCadastro = promocao.DataCadastro,
                DataInicio= promocao.DataInicio,
                DataFim = promocao.DataFim,
                PercDesconto = promocao.PercDesconto
            };
        }

        private PromocaoNovoJogoDto MapearPromocaoJogo(PrecoJogo precoJogo)
        {
            return new PromocaoNovoJogoDto
            {
                JogoId = precoJogo.JogoId,
                Valor = precoJogo.Valor,
                PromocaoId = precoJogo.PromocaoId,
                PercDesconto = precoJogo.PercDesconto,
                DataInicio = precoJogo.DataInicio,
            };
        }


    }
}
