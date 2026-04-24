using FIAP.FCG.Infrastructure.Dados.Entidades;

namespace FIAP.FCG.Infrastructure.Dados.Repositorios;

public interface IAcessoRepository
{
    Task AdicionarAsync(Acesso acesso);
}
