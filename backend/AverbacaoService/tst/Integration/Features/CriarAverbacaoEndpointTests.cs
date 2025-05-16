using System.Net;
using System.Net.Http.Json;
using AverbacaoService.Domain.Averbacoes.Features.Criar.Application;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AverbacaoService.Tests.Integration.Features;

[Collection("Integration")]
public class CriarAverbacaoEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public CriarAverbacaoEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Post_WithValidRequest_ShouldCreateAverbacao()
    {
        // Arrange
        var request = new CriarAverbacaoRequest(
            98000,
            "INSS",
            new ProponenteDto("11111111111", "Joana", "Silva", new DateTime(1980, 1, 1)),
            50000.00m,
            36
        );

        // Act
        var response = await _client.PostAsJsonAsync("/averbacoes/criar", request);

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("\"status\":{\"label\":\"CRIADA\"},\"proposta\":{\"codigo\":98000,\"convenio\":{\"nome\":\"INSS\"}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("123456789")] // CPF com menos de 11 dígitos
    [InlineData("")]
    public async Task Post_WithInvalidCpf_ShouldReturnBadRequest(string cpf)
    {
        // Arrange
        var request = new CriarAverbacaoRequest(
            98001,
            "INSS",
            new ProponenteDto(cpf, "João", "Silva", new DateTime(1980, 1, 1)),
            50000.00m,
            36
        );

        // Act
        var response = await _client.PostAsJsonAsync("/averbacoes/criar", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Invalid message. Cannot process");
    }
}
