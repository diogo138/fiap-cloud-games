using FIAP.FCG.Application.Promocoes.Dtos;

namespace FIAP.FCG.Application.Promocoes.Services
{
    public interface IPromocoesService
    {
        Task<IEnumerable<PromocaoDto>> ListarPromocoesAsync();

        Task<PromocaoDto> AtualizarPromocaoAsync(int promocaoId, PromocaoAtualizarDto promocaoAtualizarDto);

        Task<PromocaoDto> ObterPromocaoPorIdAsync(int promocaoId);

        Task<PromocaoDto> AdicionarPromocaoAsync(PromocaoNovaDto promocaoNovaDto);

        Task RemoverPromocaoAsync(int promocaoId);

        Task<PromocaoNovoJogoDto> AdicionarNovoJogoEmPromocaoAsync(int promocaoId, int jogoId);

        Task RemoverNovoJogoEmPromocaoAsync(int promocaoId, int jogoId);
    }
}
