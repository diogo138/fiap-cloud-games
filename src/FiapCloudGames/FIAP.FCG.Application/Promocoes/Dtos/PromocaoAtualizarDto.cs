namespace FIAP.FCG.Application.Promocoes.Dtos
{
    public class PromocaoAtualizarDto
    {
        public string Nome { get; set; } = string.Empty;
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public decimal PercDesconto { get; set; }
    }
}
