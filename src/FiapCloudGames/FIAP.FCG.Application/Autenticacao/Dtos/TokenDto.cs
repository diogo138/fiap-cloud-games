namespace FIAP.FCG.Application.Autenticacao.Dtos;

public class TokenDto
{
    public string Token { get; set; } = null!;
    public DateTime Expiracao { get; set; }
}
