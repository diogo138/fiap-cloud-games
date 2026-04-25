using FIAP.FCG.Infrastructure.Dados.Entidades;

namespace FIAP.FCG.Infrastructure.Dados.Repositorios
{
    public interface IListaDeDesejosRepository
    {
        Task<Usuario> ObterListaDeDesejosPorUsuarioAsync(int usuarioId);

        Task AdicionarNovoJogoEmListaDeDesejosAsync(int usuarioId, int jogoId);

        Task RemoverJogoDaListaDeDesejosAsync(int usuarioId, int jogoId);
    }
}
