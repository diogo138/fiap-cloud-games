namespace FIAP.FCG.Infrastructure.Dados.Entidades;

public partial class Jogo
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public int CategoriaId { get; set; }

    public string? Descricao { get; set; }

    public DateTime DataCadastro { get; set; }

    public DateOnly? DataLancamento { get; set; }

    public bool Visivel { get; set; }

    public bool Ativo { get; set; }

    public virtual ICollection<Carrinho> Carrinhos { get; set; } = new List<Carrinho>();

    public virtual Categoria Categoria { get; set; } = null!;

    public virtual ICollection<JogoAdquirido> JogosAdquiridos { get; set; } = new List<JogoAdquirido>();

    public virtual ICollection<PrecoJogo> PrecosJogos { get; set; } = new List<PrecoJogo>();

    public virtual ICollection<Resource> Resources { get; set; } = new List<Resource>();

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
