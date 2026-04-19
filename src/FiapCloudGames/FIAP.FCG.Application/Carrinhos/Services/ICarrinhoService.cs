using FIAP.FCG.Application.Carrinhos.Dtos;

namespace FIAP.FCG.Application.Carrinhos.Services;

public interface ICarrinhoService
{

	Task<IEnumerable<CarrinhoDto>> ListarPorUsuarioAsync(int usuario);

	Task<CarrinhoDto> AdicionarAsync(CarrinhoNovoDto dto);

	Task<CarrinhoDto> AtualizarQuantidadeAsync(int id, CarrinhoAtualizadoDto dto);

}
