namespace FIAP.FCG.Application.Usuarios.Dtos;

public class UsuarioDto
{
    public int Id { get; set; }
    public string NomeUsuario { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; }
}
