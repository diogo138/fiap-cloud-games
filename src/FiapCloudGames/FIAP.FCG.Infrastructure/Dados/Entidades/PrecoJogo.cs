namespace FIAP.FCG.Infrastructure.Dados.Entidades;

public partial class PrecoJogo
{
    public int Id { get; set; }

    public int JogoId { get; set; }

    public decimal Valor { get; set; }

    public int? PromocaoId { get; set; }

    public decimal? PercDesconto { get; set; }

    public DateTime DataInicio { get; set; }

    public virtual Jogo Jogo { get; set; } = null!;

    public virtual Promocao? Promocao { get; set; }
}
