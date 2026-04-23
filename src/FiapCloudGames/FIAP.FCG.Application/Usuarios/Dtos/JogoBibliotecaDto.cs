namespace FIAP.FCG.Application.Usuarios.Dtos
{
	public class JogoBibliotecaDto
	{
		public int Id { get; set; }

		public string Nome { get; set; } = null!;

		public int CategoriaId { get; set; }

		public string NomeCategoria { get; set; } = null!;

		public DateOnly? DataLancamento { get; set; }

		public bool Ativo { get; set; }

		public short Quantidade { get; set; }

	}
}
