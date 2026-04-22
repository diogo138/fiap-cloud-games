namespace FIAP.FCG.Application.Jogos.Dtos;

public class JogoNovoDto
{

	public string Nome { get; set; } = null!;

	public int CategoriaId { get; set; }

	public string? Descricao { get; set; }

	public DateOnly? DataLancamento { get; set; }

	public bool Visivel { get; set; } = true;

}
