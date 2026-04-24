using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.FCG.Web.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PadraoController : ControllerBase
{

}
