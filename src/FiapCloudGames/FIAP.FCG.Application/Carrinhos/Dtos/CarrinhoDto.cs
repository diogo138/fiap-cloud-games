namespace FIAP.FCG.Application.Carrinhos.Dtos;

public class CarrinhoDto
{

	public int Id { get; set; }

	public int UsuarioId { get; set; }

	public int JogoId { get; set; }

	public short Quantidade { get; set; }

	public DateTime DataCadastro { get; set; }

}
