namespace FIAP.FCG.Infrastructure.Dados.Entidades;

public partial class Promocao
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public DateTime DataCadastro { get; set; }

    public DateTime DataInicio { get; set; }

    public DateTime DataFim { get; set; }

    public decimal PercDesconto { get; set; }

    public virtual ICollection<PrecoJogo> PrecosJogos { get; set; } = new List<PrecoJogo>();
}
