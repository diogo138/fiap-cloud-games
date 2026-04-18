namespace FIAP.FCG.Infrastructure.Dados.Entidades;

public partial class Carrinho
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public int JogoId { get; set; }

    public short Quantidade { get; set; }

    public DateTime DataCadastro { get; set; }

    public virtual Jogo Jogo { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
