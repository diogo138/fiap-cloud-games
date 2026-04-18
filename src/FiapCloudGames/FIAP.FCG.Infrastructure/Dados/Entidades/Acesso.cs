namespace FIAP.FCG.Infrastructure.Dados.Entidades;

public partial class Acesso
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public DateTime DataHora { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
