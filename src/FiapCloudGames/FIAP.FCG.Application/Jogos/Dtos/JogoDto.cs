namespace FIAP.FCG.Application.Jogos.Dtos;

public class JogoDto
{

	public int Id { get; set; }

	public string Nome { get; set; } = null!;

	public int CategoriaId { get; set; }

	public string? Descricao { get; set; }

	public DateTime DataCadastro { get; set; }

	public DateOnly? DataLancamento { get; set; }

	public bool Visivel { get; set; }

	public bool Ativo { get; set; }

}
