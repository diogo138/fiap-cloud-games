namespace FIAP.FCG.Infrastructure.Dados.Entidades;

public partial class Resource
{
    public int Id { get; set; }

    public int JogoId { get; set; }

    public string TipoResource { get; set; } = null!;

    public string? Descricao { get; set; }

    public string Endereco { get; set; } = null!;

    public DateTime DataCadastro { get; set; }

    public bool Ativo { get; set; }

    public virtual Jogo Jogo { get; set; } = null!;
}
