using AverbacaoService.Domain.Averbacoes;
using AverbacaoService.Domain.Averbacoes.Features.Criar;
using AverbacaoService.Domain.Averbacoes.Features.Criar.Application;
using AverbacaoService.Domain.shared.ValueObjects;
using AverbacaoService.shared;
using CSharpFunctionalExtensions;
using FastEndpoints;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace AverbacaoService.Tests.Unit.Endpoints;

public class CriarAverbacaoEndpointTests
{
    private readonly Mock<ILogger<CriarEndpoint>> _loggerMock;
    private readonly Mock<CriarCommandHandler> _handlerMock;
    private readonly Mock<HttpResponseFactory> _httpResponseFactoryMock;
    private readonly CriarEndpoint _endpoint;

    public CriarAverbacaoEndpointTests()
    {
        _loggerMock = new Mock<ILogger<CriarEndpoint>>();
        var repositoryMock = new Mock<IAverbacoesRepository>();
        _handlerMock = new Mock<CriarCommandHandler>(repositoryMock.Object);
        _httpResponseFactoryMock = new Mock<HttpResponseFactory>();
        _endpoint = new CriarEndpoint(_loggerMock.Object, _handlerMock.Object, _httpResponseFactoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidRequest_ShouldCreateAverbacao()
    {
        // Arrange
        var request = new CriarAverbacaoRequest(
            12345,
            "INSS",
            new ProponenteDto("12345678900", "João", "Silva", new DateTime(1980, 1, 1)),
            50000.00m,
            36
        );

        var expectedAverbacao = Averbacao.Criar(new Proposta(
            request.Codigo,
            Convenio.Inss,
            new Proponente(Cpf.Criar(request.Proponente.Cpf).Value, request.Proponente.Nome, request.Proponente.Sobrenome, request.Proponente.DataNascimento),
            request.Valor,
            new Prazo(request.PrazoEmMeses)
        )).Value;

        _handlerMock.Setup(h => h.HandleAsync(It.IsAny<CriarCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(expectedAverbacao));

        var successResult = Results.Ok(new { Status = "Success" });
        _httpResponseFactoryMock.Setup(f => f.CreateSuccess200(It.IsAny<string>()))
            .Returns(successResult);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _handlerMock.Verify(h => h.HandleAsync(
            It.Is<CriarCommand>(c =>
                c.Proposta.Codigo == request.Codigo &&
                c.Proposta.Proponente.Cpf.Valor == request.Proponente.Cpf &&
                c.Proposta.Proponente.Nome == request.Proponente.Nome &&
                c.Proposta.Proponente.Sobrenome == request.Proponente.Sobrenome &&
                c.Proposta.Proponente.DataNascimento == request.Proponente.DataNascimento &&
                c.Proposta.Valor == request.Valor &&
                c.Proposta.Prazo.Meses == request.PrazoEmMeses
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        _httpResponseFactoryMock.Verify(f => f.CreateSuccess200(It.IsAny<string>()), Times.Once);
    }

    [Theory]
    [InlineData("123456789")] // CPF com menos de 11 dígitos
    [InlineData("")]
    public async Task HandleAsync_WithInvalidCpf_ShouldReturnBadRequest(string cpf)
    {
        // Arrange
        var request = new CriarAverbacaoRequest(
            12345,
            "INSS",
            new ProponenteDto(cpf, "João", "Silva", new DateTime(1980, 1, 1)),
            50000.00m,
            36
        );

        var errorResult = Results.BadRequest(new { Status = "Error" });
        _httpResponseFactoryMock.Setup(f => f.CreateError400(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(errorResult);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _httpResponseFactoryMock.Verify(f => f.CreateError400(
            "Invalid message. Cannot process.",
            It.IsAny<string>()
        ), Times.Once);

        _handlerMock.Verify(h => h.HandleAsync(It.IsAny<CriarCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
