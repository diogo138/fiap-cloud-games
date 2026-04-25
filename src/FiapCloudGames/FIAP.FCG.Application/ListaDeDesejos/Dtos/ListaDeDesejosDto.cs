namespace FIAP.FCG.Application.ListaDeDesejos.Dtos
{
    public class ListaDeDesejosDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string? Descricao { get; set; }
        public DateOnly? DataLancamento { get; set; }
    }
}
