using System.Net;
using System.Text.Json;

namespace FIAP.FCG.Web.API.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _proximo;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate proximo, ILogger<ErrorHandlingMiddleware> logger)
    {
        _proximo = proximo;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await _proximo(contexto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro não tratado na requisição: {Metodo} {Caminho}",
                contexto.Request.Method,
                contexto.Request.Path);

            await TratarExcecaoAsync(contexto, ex);
        }
    }

    private static async Task TratarExcecaoAsync(HttpContext contexto, Exception excecao)
    {
        contexto.Response.ContentType = "application/json";

        var (statusCode, mensagem) = excecao switch
        {
            ArgumentException => (HttpStatusCode.BadRequest, excecao.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Acesso não autorizado."),
            KeyNotFoundException => (HttpStatusCode.NotFound, excecao.Message),
            _ => (HttpStatusCode.InternalServerError, "Ocorreu um erro interno. Tente novamente mais tarde.")
        };

        contexto.Response.StatusCode = (int)statusCode;

        var resposta = new
        {
            status = (int)statusCode,
            erro = mensagem
        };

        var json = JsonSerializer.Serialize(resposta, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await contexto.Response.WriteAsync(json);
    }
}
