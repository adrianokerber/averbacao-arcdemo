using AverbacaoService.shared;
using FastEndpoints;

namespace AverbacaoService.Domain.Averbacoes.Features.Formalizar.Application;

public class FormalizarEndpoint(ILogger<FormalizarEndpoint> logger, FormalizarCommandHandler handler, HttpResponseFactory httpResponseFactory) : Endpoint<FormalizarRequest, object>
{
    public override void Configure()
    {
        Post("/averbacoes/formalizar");
        AllowAnonymous();
    }

    public override async Task HandleAsync(FormalizarRequest req, CancellationToken ct)
    {
        var command = FormalizarCommand.Criar(req.CodigoProposta);
        if (command.IsFailure)
        {
            await SendResultAsync(httpResponseFactory.CreateError400("Invalid message. Cannot process.", command.Error));
            return;
        }

        var result = await handler.HandleAsync(command.Value, ct);
        if (result.IsFailure)
        {
            await SendResultAsync(httpResponseFactory.CreateError400("Invalid message. Cannot process.", result.Error));
            return;
        }

        logger.LogInformation("Averbacao formalizada com sucesso");
        await SendResultAsync(httpResponseFactory.CreateSuccess200(result.Value));
    }
}

public record FormalizarRequest
{
    public int CodigoProposta { get; set; }
}