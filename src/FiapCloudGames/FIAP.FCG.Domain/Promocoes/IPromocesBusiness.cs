namespace FIAP.FCG.Domain.Promocoes
{
    public interface IPromocesBusiness
    {
        Task<bool> PromocaoJaExiste(string nome);

        bool PeriodoPromocaoInvalido(DateTime dataInicio, DateTime dataFim);
    }
}
