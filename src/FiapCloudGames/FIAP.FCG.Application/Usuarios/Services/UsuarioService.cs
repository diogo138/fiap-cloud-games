using System.Security.Cryptography;
using System.Text;
using FIAP.FCG.Application.Jogos.Dtos;
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

	public async Task<IEnumerable<JogoBibliotecaDto>> ListarBibliotecaAsync(int usuarioId)
	{
		var usuario = await _repositorio.ConsultarBibliotecaAsync(usuarioId);

		if (usuario == null)
			throw new KeyNotFoundException("Usuário não encontrado.");

		return MapegarJogoDaBiblioteca(usuario);
	}

	private IEnumerable<JogoBibliotecaDto> MapegarJogoDaBiblioteca(Usuario usuario) => 
		usuario.JogosAdquiridos.Select(MapegarJogoAdquirido);

	private static JogoBibliotecaDto MapegarJogoAdquirido(JogoAdquirido j)
	{
		return new JogoBibliotecaDto()
		{
			Id = j.Jogo.Id,
			Nome = j.Jogo.Nome,
			CategoriaId = j.Jogo.CategoriaId,
			NomeCategoria = j.Jogo.Categoria.Nome,
			DataLancamento = j.Jogo.DataLancamento,
			Ativo = j.Jogo.Ativo,
			Quantidade = j.Quantidade,
		};
	}

	public async Task<JogoDetalhadoBibliotecaDto> ListarBibliotecaAsync(int usuarioId, int jogoId)
	{
		var usuario = await _repositorio.ConsultarBibliotecaAsync(usuarioId);
		if (usuario == null)
			throw new KeyNotFoundException("Usuário não encontrado.");

		var jogoAdquirido = usuario.JogosAdquiridos.FirstOrDefault(ja => ja.JogoId == jogoId);
		if (jogoAdquirido == null)
			throw new KeyNotFoundException("Jogo não encontrado na biblioteca do usuário.");

		return MapearJogoAdquiridoDetalhado(jogoAdquirido);
	}

	private static JogoDetalhadoBibliotecaDto MapearJogoAdquiridoDetalhado(JogoAdquirido j)
	{
		return new JogoDetalhadoBibliotecaDto()
		{
			Id = j.Jogo.Id,
			Nome = j.Jogo.Nome,
			CategoriaId = j.Jogo.CategoriaId,
			NomeCategoria = j.Jogo.Categoria.Nome,
			Descricao = j.Jogo.Descricao,
			DataCadastro = j.Jogo.DataCadastro,
			DataLancamento = j.Jogo.DataLancamento,
			Ativo = j.Jogo.Ativo,
			Quantidade = j.Quantidade,
			ValorUnitario = j.ValorUnitario,
			ValorTotal = j.ValorTotal
		};
	}

}
