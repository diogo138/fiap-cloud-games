using FIAP.FCG.Application.Categorias.Dtos;

namespace FIAP.FCG.Application.Categorias.Services;

public interface ICategoriaService
{

	Task<IEnumerable<CategoriaDto>> ListarAsync(bool? ativo, string? q);

	Task<CategoriaDto> ConsultarAsync(int id);

	Task<CategoriaDto> AdicionarAsync(CategoriaNovoDto dto);

	Task<CategoriaDto> AtualizarAsync(int id, CategoriaAtualizadoDto dto);

	Task RemoverAsync(int id);

}
