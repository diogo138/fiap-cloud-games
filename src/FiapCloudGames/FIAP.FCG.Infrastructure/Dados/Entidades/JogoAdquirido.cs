namespace FIAP.FCG.Infrastructure.Dados.Entidades;

public partial class JogoAdquirido
{
    public int Id { get; set; }

    public int TransacaoId { get; set; }

    public int UsuarioId { get; set; }

    public int JogoId { get; set; }

    public short Quantidade { get; set; }

    public decimal ValorUnitario { get; set; }

    public decimal ValorTotal { get; set; }

    public virtual Jogo Jogo { get; set; } = null!;

    public virtual Transacao Transacao { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
