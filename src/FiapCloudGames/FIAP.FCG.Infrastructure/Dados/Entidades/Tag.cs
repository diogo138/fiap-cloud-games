namespace FIAP.FCG.Infrastructure.Dados.Entidades;

public partial class Tag
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public DateTime DataCadastro { get; set; }

    public bool Ativo { get; set; }

    public virtual ICollection<Jogo> Jogos { get; set; } = new List<Jogo>();
}
