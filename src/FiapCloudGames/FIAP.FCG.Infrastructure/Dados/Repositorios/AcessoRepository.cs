using FIAP.FCG.Infrastructure.Dados.Entidades;
using Microsoft.EntityFrameworkCore;

namespace FIAP.FCG.Infrastructure.Dados.Repositorios;

public class AcessoRepository : IAcessoRepository
{
    private readonly DbSet<Acesso> _repositorio;

    public AcessoRepository(IUnidadeDeTrabalho unidade)
    {
        _repositorio = unidade.Repositorio<Acesso>();
    }

    public async Task AdicionarAsync(Acesso acesso)
        => await _repositorio.AddAsync(acesso);
}
