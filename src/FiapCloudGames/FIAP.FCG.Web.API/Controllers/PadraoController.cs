using FIAP.FCG.Application.Autenticacao.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.FCG.Web.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PadraoController : ControllerBase
{

	private ContextoAtual? _contexto;

	protected ContextoAtual ContextoSeguranca
	{
		get => (_contexto ??= ObterContextoSeguranca());
	}

	private ContextoAtual ObterContextoSeguranca()
	{
		var usuarioId = int.Parse(User.Claims.First(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value);
		var email = User.Claims.First(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
		var administrador = (User.Claims.First(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value == "Administrador");
		return new ContextoAtual(usuarioId, email, administrador);
	}
}
