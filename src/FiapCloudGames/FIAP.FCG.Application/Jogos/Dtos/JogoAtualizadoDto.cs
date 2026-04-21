namespace FIAP.FCG.Application.Jogos.Dtos;

public class JogoAtualizadoDto
{

	public string Nome { get; set; } = null!;

	public int CategoriaId { get; set; }

	public string? Descricao { get; set; }

	public DateOnly? DataLancamento { get; set; }

	public bool Visivel { get; set; }

	public bool Ativo { get; set; }

}
