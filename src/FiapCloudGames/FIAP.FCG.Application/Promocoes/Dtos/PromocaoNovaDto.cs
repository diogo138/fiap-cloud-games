namespace FIAP.FCG.Application.Promocoes.Dtos
{
    public class PromocaoNovaDto
    {
        public string Nome { get; set; } = null!;

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }

        public decimal PercDesconto { get; set; }
    }
}
