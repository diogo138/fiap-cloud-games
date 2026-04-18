namespace FIAP.FCG.Infrastructure.Dados.Entidades;

public partial class Transacao
{
    public int Id { get; set; }

    public int IntegradoraPagamentoId { get; set; }

    public DateTime DataCompra { get; set; }

    public decimal ValorTotal { get; set; }

    public string? Comprovante { get; set; }

    public virtual IntegradoraPagamento IntegradoraPagamento { get; set; } = null!;

    public virtual ICollection<JogoAdquirido> JogosAdquiridos { get; set; } = new List<JogoAdquirido>();
}
