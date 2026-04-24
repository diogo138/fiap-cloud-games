using FIAP.FCG.Infrastructure.Dados.Entidades;
using Microsoft.EntityFrameworkCore;

namespace FIAP.FCG.Infrastructure.Dados.Repositorios
{
    public class ListaDeDesejosRepository : IListaDeDesejosRepository
    {
        private readonly DbSet<Categoria> _repositorio;
        /*
        public ListaDeDesejosRepository(IUnidadeDeTrabalho unidade)
        {
            _repositorio = unidade.Repositorio<Categoria>();
        }

        public Task<ListaDeDesejosDto> ObterListaDeDesejosPorUsuarioAsync(int usuarioId)
        {
            
        }
        */
    }
}
