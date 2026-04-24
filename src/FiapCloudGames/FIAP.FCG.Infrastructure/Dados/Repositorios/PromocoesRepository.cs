using FIAP.FCG.Infrastructure.Dados.Entidades;
using Microsoft.EntityFrameworkCore;

namespace FIAP.FCG.Infrastructure.Dados.Repositorios
{
    public class PromocoesRepository : IPromocoesRepository
    {
        private readonly DbSet<Promocao> _repositorio;

        public PromocoesRepository(IUnidadeDeTrabalho unidade)
        {
            _repositorio = unidade.Repositorio<Promocao>();
        }

        public async Task<IEnumerable<Promocao?>> ConsultarPromocoesAsync()
            => await _repositorio.ToListAsync();

        public async Task<Promocao?> ObterPromocaoPorIdAsync(int promocoesId)
            => await _repositorio.FindAsync(promocoesId);

        public async Task AdicionarPromocaoAsync(Promocao promocao)
            => await _repositorio.AddAsync(promocao);

        public async Task RemoverPromocaoAsync(int promocaoId)
        {
            var promocao = await _repositorio.FindAsync(promocaoId);
            _repositorio.Remove(promocao);
        }

        public Task AtualizarPromocaoAsync(Promocao promocao)
        {
            _repositorio.Update(promocao);
            return Task.CompletedTask;
        }
    }
}
