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

        public async Task<IEnumerable<Promocao>> ConsultarPromocoesAsync(string? nome, int page, string? orderBy, bool desc)
        {
            var query = _repositorio.AsQueryable();

            // 🔍 Filtro por nome
            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.Nome.Contains(nome));
            }

            // 🔄 Ordenação
            query = orderBy?.ToLower() switch
            {
                "nome" => desc
                    ? query.OrderByDescending(p => p.Nome)
                    : query.OrderBy(p => p.Nome),

                "datainicio" => desc
                    ? query.OrderByDescending(p => p.DataInicio)
                    : query.OrderBy(p => p.DataInicio),

                _ => desc
                    ? query.OrderByDescending(p => p.DataCadastro)
                    : query.OrderBy(p => p.DataCadastro)
            };

            // 📄 Paginação (10 por página)
            const int pageSize = 10;

            query = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return await query.ToListAsync();
        }

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
