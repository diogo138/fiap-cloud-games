namespace FIAP.FCG.Application.Promocoes.Dtos
{
    public class PromocaoDto
    {
        public int Id { get; set; }

        public string Nome { get; set; } = null!;

        public DateTime DataCadastro { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }

        public decimal PercDesconto { get; set; }
    }
}
