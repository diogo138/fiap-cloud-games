using System.Security.Cryptography;
using System.Text;
using FIAP.FCG.Application.Usuarios.Dtos;
using FIAP.FCG.Domain.Usuarios;
using FIAP.FCG.Infrastructure.Dados.Entidades;
using FIAP.FCG.Infrastructure.Dados.Repositorios;

namespace FIAP.FCG.Application.Usuarios.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repositorio;
    private readonly IUsuarioBusiness _business;
    private readonly IAdministradorRepository _administradorRepositorio;

    public UsuarioService(IUsuarioRepository repositorio, IUsuarioBusiness business, IAdministradorRepository administradorRepositorio)
    {
        _repositorio = repositorio;
        _business = business;
        _administradorRepositorio = administradorRepositorio;
    }

    public async Task<IEnumerable<UsuarioDto>> ListarAsync()
    {
        var usuarios = await _repositorio.ListarAsync();
        return usuarios.Select(MapearUsuario);
    }

    public async Task<UsuarioDto> ConsultarAsync(int id)
    {
        var usuario = await _repositorio.ConsultarAsync(id);

        if (usuario == null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        return MapearUsuario(usuario);
    }

    public async Task<UsuarioDto> AdicionarAsync(UsuarioNovoDto dto)
    {
        if (await _business.EmailJaCadastrado(dto.Email))
            throw new InvalidOperationException("E-mail já cadastrado.");

        var usuario = new Usuario
        {
            NomeUsuario = dto.NomeUsuario,
            Email = dto.Email,
            HashSenha = GerarHash(dto.Senha),
            DataCadastro = DateTime.UtcNow,
            Ativo = true
        };

        await _repositorio.AdicionarAsync(usuario);

        return MapearUsuario(usuario);
    }

    public async Task<UsuarioDto> AtualizarAsync(int id, UsuarioAtualizadoDto dto)
    {
        var usuario = await _repositorio.ConsultarAsync(id);

        if (usuario == null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        usuario.NomeUsuario = dto.NomeUsuario;
        usuario.Email = dto.Email;

        await _repositorio.AtualizarAsync(usuario);

        return MapearUsuario(usuario);
    }

    public async Task RemoverAsync(int id)
    {
        var usuario = await _repositorio.ConsultarAsync(id);

        if (usuario == null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        usuario.Ativo = false;

        await _repositorio.AtualizarAsync(usuario);
    }

    public async Task ConcederAdminAsync(int id)
    {
        if (!await _business.UsuarioExiste(id))
            throw new KeyNotFoundException("Usuário não encontrado.");

        if (await _administradorRepositorio.ConsultarAsync(id) != null)
            throw new InvalidOperationException("Usuário já é administrador.");

        await _administradorRepositorio.AdicionarAsync(new Administrador
        {
            UsuarioId = id,
            DataCadastro = DateTime.UtcNow
        });
    }

    public async Task RevogarAdminAsync(int id)
    {
        var admin = await _administradorRepositorio.ConsultarAsync(id);

        if (admin == null)
            throw new KeyNotFoundException("Usuário não é administrador.");

        await _administradorRepositorio.RemoverAsync(admin);
    }

    private static UsuarioDto MapearUsuario(Usuario usuario) => new()
    {
        Id = usuario.Id,
        NomeUsuario = usuario.NomeUsuario,
        Email = usuario.Email,
        DataCadastro = usuario.DataCadastro,
        Ativo = usuario.Ativo
    };

    private static string GerarHash(string senha)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(senha));
        return Convert.ToHexString(bytes).ToLower();
    }
}
