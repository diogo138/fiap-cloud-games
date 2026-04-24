using FIAP.FCG.Infrastructure.Dados.Repositorios;

namespace FIAP.FCG.Domain.Promocoes
{
    public class PromocesBusiness : IPromocesBusiness
    {
        private readonly IPromocoesRepository _repositorio;

        public PromocesBusiness(IPromocoesRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public bool PeriodoPromocaoInvalido(DateTime dataInicio, DateTime dataFim)
        {
            return dataFim.Date <= dataInicio.Date || dataInicio.Date < DateTime.UtcNow.Date;
        }

        public async Task<bool> PromocaoJaExiste(string nome)
        {
            var promocoes = await _repositorio.ConsultarPromocoesAsync();
            return promocoes.Any(p => p != null && p.Nome == nome);
        }
    }
}
