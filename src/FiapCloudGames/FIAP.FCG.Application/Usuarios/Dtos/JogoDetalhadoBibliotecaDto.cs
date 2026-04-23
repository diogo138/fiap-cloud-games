namespace FIAP.FCG.Application.Usuarios.Dtos
{
	public class JogoDetalhadoBibliotecaDto : JogoBibliotecaDto
	{

		public string? Descricao { get; set; }

		public DateTime DataCadastro { get; set; }

		public decimal ValorUnitario { get; set; }

		public decimal ValorTotal { get; set; }

	}
}
