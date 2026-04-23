using FIAP.FCG.Infrastructure.Dados.Entidades;

namespace FIAP.FCG.Infrastructure.Dados.Repositorios
{
    public interface IPromocoesRespository
    {
        Task<IEnumerable<Promocao?>> ConsultarPromocoesAsync();
        Task<Promocao?> ObterPromocaoPorIdAsync(int promocoesId); 

        Task AdicionarPromocaoAsync(Promocao promocao);

        Task RemoverPromocaoAsync(int promocaoId);
    }
}
