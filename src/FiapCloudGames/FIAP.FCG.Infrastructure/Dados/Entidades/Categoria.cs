namespace FIAP.FCG.Infrastructure.Dados.Entidades;

public partial class Categoria
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string? Descricao { get; set; }

    public DateTime DataCadastro { get; set; }

    public bool Ativo { get; set; }

    public virtual ICollection<Jogo> Jogos { get; set; } = new List<Jogo>();
}
