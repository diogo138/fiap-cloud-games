using FIAP.FCG.Application.Jogos.Dtos;

namespace FIAP.FCG.Application.Jogos.Services;

public interface IJogoService
{

	Task<IEnumerable<JogoDto>> ListarAsync(int? categoriaId, bool? visivel, bool? ativo, string? q);

	Task<JogoDto> ConsultarAsync(int id);

	Task<JogoDto> AdicionarAsync(JogoNovoDto dto);

	Task<JogoDto> AtualizarAsync(int id, JogoAtualizadoDto dto);

	Task RemoverAsync(int id);

	Task AjustarPrecoAsync(int id, AjustarPrecoDto dto);

}
