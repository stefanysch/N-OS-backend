using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using N_OS.Application.Exceptions;

namespace N_OS.API.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetails;
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandler(
        IProblemDetailsService problemDetails,
        ILogger<GlobalExceptionHandler> logger,
        IHostEnvironment environment)
    {
        _problemDetails = problemDetails;
        _logger = logger;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, titulo, mensagem) = exception switch
        {
            ArgumentException =>
                (StatusCodes.Status400BadRequest, "Requisição inválida", exception.Message),

            NaoEncontradoException =>
                (StatusCodes.Status404NotFound, "Recurso não encontrado", exception.Message),

            CredenciaisInvalidasException =>
                (StatusCodes.Status401Unauthorized, "Não autenticado", exception.Message),

            RegraDeNegocioException =>
                (StatusCodes.Status409Conflict, "Conflito com o estado atual", exception.Message),

            BadHttpRequestException =>
                (StatusCodes.Status400BadRequest, "Requisição inválida", "Corpo da requisição inválido."),

            _ =>
                (StatusCodes.Status500InternalServerError, "Erro interno",
                    "Ocorreu um erro inesperado. Tente novamente mais tarde."),
        };

        if (status >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "Erro não tratado em {Metodo} {Caminho}",
                httpContext.Request.Method,
                httpContext.Request.Path);
        }

        httpContext.Response.StatusCode = status;

        var problema = new ProblemDetails
        {
            Status = status,
            Title = titulo,
            Detail = mensagem,
        };

        if (status >= StatusCodes.Status500InternalServerError &&
            _environment.IsDevelopment())
        {
            problema.Extensions["excecao"] = exception.ToString();
        }

        return await _problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problema,
            Exception = exception,
        });
    }
}
