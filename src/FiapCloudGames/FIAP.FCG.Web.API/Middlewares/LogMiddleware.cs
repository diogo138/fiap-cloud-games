namespace FIAP.FCG.Web.API.Middlewares;

public class LogMiddleware
{
    private readonly RequestDelegate _proximo;
    private readonly ILogger<LogMiddleware> _logger;

    public LogMiddleware(RequestDelegate proximo, ILogger<LogMiddleware> logger)
    {
        _proximo = proximo;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        var inicio = DateTime.UtcNow;

        _logger.LogInformation(
            "Requisição recebida: {Metodo} {Caminho} | IP: {Ip}",
            contexto.Request.Method,
            contexto.Request.Path,
            contexto.Connection.RemoteIpAddress);

        await _proximo(contexto);

        var duracao = DateTime.UtcNow - inicio;

        _logger.LogInformation(
            "Requisição concluída: {Metodo} {Caminho} | Status: {StatusCode} | Duração: {Duracao}ms",
            contexto.Request.Method,
            contexto.Request.Path,
            contexto.Response.StatusCode,
            duracao.TotalMilliseconds);
    }
}
