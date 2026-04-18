namespace FIAP.FCG.Infrastructure.Dados.Entidades;

public partial class Administrador
{
    public int UsuarioId { get; set; }

    public DateTime DataCadastro { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
