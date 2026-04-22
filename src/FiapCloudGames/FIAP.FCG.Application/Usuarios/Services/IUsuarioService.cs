using FIAP.FCG.Application.Usuarios.Dtos;

namespace FIAP.FCG.Application.Usuarios.Services;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioDto>> ListarAsync();
    Task<UsuarioDto> ConsultarAsync(int id);
    Task<UsuarioDto> AdicionarAsync(UsuarioNovoDto dto);
    Task<UsuarioDto> AtualizarAsync(int id, UsuarioAtualizadoDto dto);
    Task RemoverAsync(int id);
    Task ConcederAdminAsync(int id);
    Task RevogarAdminAsync(int id);
}
