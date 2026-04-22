using FIAP.FCG.Infrastructure.Dados.Entidades;
using Microsoft.EntityFrameworkCore;

namespace FIAP.FCG.Infrastructure.Dados.Repositorios;

public class AdministradorRepository : IAdministradorRepository
{
    private readonly DbSet<Administrador> _repositorio;

    public AdministradorRepository(IUnidadeDeTrabalho unidade)
    {
        _repositorio = unidade.Repositorio<Administrador>();
    }

    public async Task<Administrador?> ConsultarAsync(int usuarioId)
        => await _repositorio.FindAsync(usuarioId);

    public async Task AdicionarAsync(Administrador administrador)
        => await _repositorio.AddAsync(administrador);

    public async Task RemoverAsync(Administrador administrador)
    {
        _repositorio.Remove(administrador);
        await Task.CompletedTask;
    }
}
