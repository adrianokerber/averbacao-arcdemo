using Microsoft.AspNetCore.Diagnostics;

namespace AverbacaoService.shared;

public sealed class HttpGlobalExceptionHandler(IWebHostEnvironment env, ILogger<HttpGlobalExceptionHandler> logger, HttpResponseFactory httpResponseFactory) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            "Unhandled exception occurred: {Message}",
            exception.Message);

        var errorResult = httpResponseFactory.CreateError500(env.IsDevelopment() ? exception.ToString() : "An error occurred, try again later.");
        await errorResult.ExecuteAsync(httpContext);

        return true;
    }
}