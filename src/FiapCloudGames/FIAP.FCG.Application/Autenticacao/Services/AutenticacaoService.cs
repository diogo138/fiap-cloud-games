using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FIAP.FCG.Application.Autenticacao.Dtos;
using FIAP.FCG.Infrastructure.Dados.Entidades;
using FIAP.FCG.Infrastructure.Dados.Repositorios;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FIAP.FCG.Application.Autenticacao.Services;

public class AutenticacaoService : IAutenticacaoService
{
    private readonly IUsuarioRepository _usuarioRepositorio;
    private readonly IAdministradorRepository _administradorRepositorio;
    private readonly IAcessoRepository _acessoRepositorio;
    private readonly IConfiguration _configuration;

    public AutenticacaoService(
        IUsuarioRepository usuarioRepositorio,
        IAdministradorRepository administradorRepositorio,
        IAcessoRepository acessoRepositorio,
        IConfiguration configuration)
    {
        _usuarioRepositorio = usuarioRepositorio;
        _administradorRepositorio = administradorRepositorio;
        _acessoRepositorio = acessoRepositorio;
        _configuration = configuration;
    }

    public async Task<TokenDto> AutenticarAsync(AutenticacaoDto dto)
    {
        var usuario = await _usuarioRepositorio.ConsultarPorEmailAsync(dto.Email);

        if (usuario == null || !usuario.Ativo)
            throw new UnauthorizedAccessException("Credenciais inválidas.");

        var hashSenha = GerarHash(dto.Senha);
        if (usuario.HashSenha != hashSenha)
            throw new UnauthorizedAccessException("Credenciais inválidas.");

        var eAdmin = await _administradorRepositorio.ConsultarAsync(usuario.Id) != null;
        var role = eAdmin ? "Administrador" : "Usuario";

        await _acessoRepositorio.AdicionarAsync(new Acesso
        {
            UsuarioId = usuario.Id,
            DataHora = DateTime.UtcNow
        });

        return GerarToken(usuario, role);
    }

    private TokenDto GerarToken(Usuario usuario, string role)
    {
        var chave = _configuration["Jwt:Chave"]!;
        var emissor = _configuration["Jwt:Emissor"]!;
        var audiencia = _configuration["Jwt:Audiencia"]!;
        var expiracaoHoras = int.Parse(_configuration["Jwt:ExpiracaoHoras"]!);

        var chaveBytes = Encoding.UTF8.GetBytes(chave);
        var credenciais = new SigningCredentials(
            new SymmetricSecurityKey(chaveBytes),
            SecurityAlgorithms.HmacSha256);

        var expiracao = DateTime.UtcNow.AddHours(expiracaoHoras);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: emissor,
            audience: audiencia,
            claims: claims,
            expires: expiracao,
            signingCredentials: credenciais);

        return new TokenDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiracao = expiracao
        };
    }

    private static string GerarHash(string senha)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(senha));
        return Convert.ToHexString(bytes).ToLower();
    }
}
