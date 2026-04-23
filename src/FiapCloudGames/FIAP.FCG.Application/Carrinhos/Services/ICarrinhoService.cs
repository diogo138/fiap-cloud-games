using FIAP.FCG.Application.Carrinhos.Dtos;

namespace FIAP.FCG.Application.Carrinhos.Services;

public interface ICarrinhoService
{

	Task<IEnumerable<CarrinhoDto>> ListarPorUsuarioAsync(int usuario);

	Task<CarrinhoDto> ListarPorUsuarioCarrinhoAsync(int usuarioId, int carrinhoId);

	Task<CarrinhoDto> AdicionarAsync(int usuarioId, CarrinhoNovoDto dto);

	Task<CarrinhoDto> AtualizarQuantidadeAsync(int usuarioId, int carrinhoId, CarrinhoAtualizadoDto dto);

	Task RemoverAsync(int usuarioId, int carrinhoId);

}
