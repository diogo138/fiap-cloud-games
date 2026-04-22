using FIAP.FCG.Infrastructure.Dados.Entidades;

namespace FIAP.FCG.Infrastructure.Dados.Repositorios;

public interface IAdministradorRepository
{
    Task<Administrador?> ConsultarAsync(int usuarioId);
    Task AdicionarAsync(Administrador administrador);
    Task RemoverAsync(Administrador administrador);
}
