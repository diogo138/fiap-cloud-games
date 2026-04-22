namespace FIAP.FCG.Application.Categorias.Dtos;

public class CategoriaDto
{

	public int Id { get; set; }

	public string Nome { get; set; } = null!;

	public string? Descricao { get; set; }

	public DateTime DataCadastro { get; set; }

	public bool Ativo { get; set; }

}
