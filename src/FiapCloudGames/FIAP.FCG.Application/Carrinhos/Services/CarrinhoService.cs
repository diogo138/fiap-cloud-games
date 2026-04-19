using FIAP.FCG.Application.Carrinhos.Dtos;
using FIAP.FCG.Domain.Carrinhos;
using FIAP.FCG.Infrastructure.Dados.Entidades;
using FIAP.FCG.Infrastructure.Dados.Repositorios;

namespace FIAP.FCG.Application.Carrinhos.Services;

public class CarrinhoService : ICarrinhoService
{

	private readonly ICarrinhoRepository _repositorio;
	private readonly ICarrinhoBusiness _business;

	public CarrinhoService(ICarrinhoRepository repositorio, ICarrinhoBusiness business)
	{
		_repositorio = repositorio;
		_business = business;
	}

	public async Task<IEnumerable<CarrinhoDto>> ListarPorUsuarioAsync(int usuario)
	{
		var carrinhos = await _repositorio.ConsultarUsuarioAsync(usuario);
		return carrinhos.Select(MapegarCarrinho);
	}

	private static CarrinhoDto MapegarCarrinho(Carrinho carrinho)
	{
		return new CarrinhoDto
		{
			Id = carrinho.Id,
			UsuarioId = carrinho.UsuarioId,
			JogoId = carrinho.JogoId,
			Quantidade = carrinho.Quantidade,
			DataCadastro = carrinho.DataCadastro
		};
	}

	public async Task<CarrinhoDto> AdicionarAsync(CarrinhoNovoDto dto)
	{
		if (await _business.UsuarioJaPossuiJogo(dto.UsuarioId, dto.JogoId))
			throw new InvalidOperationException("Jogo já está no carrinho deste usuário.");

		var carrinho = new Carrinho
		{
			UsuarioId = dto.UsuarioId,
			JogoId = dto.JogoId,
			Quantidade = dto.Quantidade,
			DataCadastro = DateTime.UtcNow
		};

		await _repositorio.AdicionarAsync(carrinho);

		return MapegarCarrinho(carrinho);
	}

	public async Task<CarrinhoDto> AtualizarQuantidadeAsync(int id, CarrinhoAtualizadoDto dto)
	{
		var carrinho = await _repositorio.ConsultarAsync(id);

		if (carrinho == null)
			throw new KeyNotFoundException("Carrinho não encontrado.");

		carrinho.Quantidade = dto.Quantidade;

		await _repositorio.AtualizarAsync(carrinho);

		return MapegarCarrinho(carrinho);
	}
}
