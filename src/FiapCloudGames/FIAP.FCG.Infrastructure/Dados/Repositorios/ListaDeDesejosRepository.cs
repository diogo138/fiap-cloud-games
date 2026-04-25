using FIAP.FCG.Infrastructure.Dados.Entidades;
using Microsoft.EntityFrameworkCore;

namespace FIAP.FCG.Infrastructure.Dados.Repositorios
{
    public class ListaDeDesejosRepository : IListaDeDesejosRepository
    {
        private readonly DbSet<Usuario> _repositorio;
        private readonly DbSet<Jogo> _repositorioJogo;

        
        public ListaDeDesejosRepository(IUnidadeDeTrabalho unidade)
        {
            _repositorio = unidade.Repositorio<Usuario>();
            _repositorioJogo = unidade.Repositorio<Jogo>();
        }

        public async Task<Usuario?> ObterListaDeDesejosPorUsuarioAsync(int usuarioId)
        {
            var usuario = await _repositorio
                .Include(u => u.Jogos)
                .FirstOrDefaultAsync(u => u.Id == usuarioId);
            return usuario;
        }

        public async Task AdicionarNovoJogoEmListaDeDesejosAsync(int usuarioId, int jogoId)
        {
            var usuario = await _repositorio
                .Include(u => u.Jogos)
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
                throw new KeyNotFoundException("Usuário não encontrado");

            var jogo = await _repositorioJogo
                .FirstOrDefaultAsync(j => j.Id == jogoId);

            if (jogo == null)
                throw new KeyNotFoundException("Jogo não encontrado");

            if (usuario.Jogos.Any(j => j.Id == jogoId))
                return;

            usuario.Jogos.Add(jogo);
        }

        public async Task RemoverJogoDaListaDeDesejosAsync(int usuarioId, int jogoId)
        {
            // 1. Buscar usuário com a lista carregada
            var usuario = await _repositorio
                .Include(u => u.Jogos)
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
                throw new KeyNotFoundException("Usuário não encontrado");

            // 2. Buscar o jogo dentro da lista de desejos
            var jogo = usuario.Jogos
                .FirstOrDefault(j => j.Id == jogoId);

            if (jogo == null)
                throw new KeyNotFoundException("Jogo não está na lista de desejos");

            // 3. Remover da lista (🔥 ponto principal)
            usuario.Jogos.Remove(jogo);
        }
    }
}
