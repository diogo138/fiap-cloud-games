using FIAP.FCG.Infrastructure.Dados.Entidades;

namespace FIAP.FCG.Infrastructure.Dados.Repositorios
{
    public interface IPromocoesRepository
    {
        Task<IEnumerable<Promocao?>> ConsultarPromocoesAsync();
        Task<Promocao?> ObterPromocaoPorIdAsync(int promocoesId); 

        Task AdicionarPromocaoAsync(Promocao promocao);
        Task AtualizarPromocaoAsync(Promocao promocao);

        Task RemoverPromocaoAsync(int promocaoId);

    }
}
