using FIAP.FCG.Application.Autenticacao.Dtos;

namespace FIAP.FCG.Application.Autenticacao.Services;

public interface IAutenticacaoService
{
    Task<TokenDto> AutenticarAsync(AutenticacaoDto dto);
}
