namespace FIAP.FCG.Infrastructure.Dados.Entidades;

public partial class Usuario
{
    public int Id { get; set; }

    public string NomeUsuario { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string HashSenha { get; set; } = null!;

    public DateTime DataCadastro { get; set; }

    public bool Ativo { get; set; }

    public virtual ICollection<Acesso> Acessos { get; set; } = new List<Acesso>();

    public virtual Administrador? Administradore { get; set; }

    public virtual ICollection<Carrinho> Carrinhos { get; set; } = new List<Carrinho>();

    public virtual ICollection<JogoAdquirido> JogosAdquiridos { get; set; } = new List<JogoAdquirido>();

    public virtual ICollection<Jogo> IdJogos { get; set; } = new List<Jogo>();
}
