using GestionBD.Application.Configuration;
using GestionBD.Application.DTO.OpenAI;
using GestionBD.Infrastructure.ExternalServices.OpenAI;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using System.Text.Json;

namespace GestionBD.UnitTests.Infrastructure.ExternalServices.OpenAI;

public sealed class OpenAIRollbackGenerationServiceTests
{
    [Fact]
    public void Constructor_WithMissingModel_ShouldThrowInvalidOperationException()
    {
        var settings = Options.Create(new OpenIASettings
        {
            ApiKey = "key",
            BaseURL = "https://openai.local",
            Model = ""
        });

        var httpClient = new HttpClient(new FakeHttpMessageHandler());

        Assert.Throws<InvalidOperationException>(() =>
            new OpenAIRollbackGenerationService(httpClient, settings, NullLogger<OpenAIRollbackGenerationService>.Instance));
    }

    [Fact]
    public async Task GenerateRollbackAsync_WithEmptyDefinitions_ShouldThrowArgumentException()
    {
        var service = CreateService(out _);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GenerateRollbackAsync(string.Empty, "current"));

        Assert.Equal("newObjectsDefinitions", exception.ParamName);
    }

    [Fact]
    public async Task GenerateRollbackAsync_WithValidResponse_ShouldReturnRollbackGeneration()
    {
        var service = CreateService(out var handler);

        var rollbackJson = JsonSerializer.Serialize(new
        {
            metadata = new { engine = "sqlserver", rollbackStrategy = "full", generatedAt = "now" },
            rollbackScripts = new[]
            {
                new
                {
                    fileName = "rollback.sql",
                    objectType = "TABLE",
                    objectName = "dbo.Test",
                    rollbackOrder = 1,
                    script = "SELECT 1;",
                    dependsOn = new[] { "dbo.Other" }
                }
            },
            warnings = new[] { "warn" },
            assumptions = new[] { "assume" }
        }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        handler.Response = CreateOpenAIResponse(rollbackJson);

        var result = await service.GenerateRollbackAsync("new", "current");

        Assert.Equal(1, result.ScriptCount);
        Assert.True(result.HasWarnings);
        Assert.True(result.HasAssumptions);
        Assert.Equal("/v1/responses", handler.LastRequest?.RequestUri?.AbsolutePath);
    }

    private static OpenAIRollbackGenerationService CreateService(out FakeHttpMessageHandler handler)
    {
        var settings = Options.Create(new OpenIASettings
        {
            ApiKey = "key",
            BaseURL = "https://openai.local",
            Model = "gpt-4",
            MaxTokens = "600"
        });

        handler = new FakeHttpMessageHandler();
        var httpClient = new HttpClient(handler);

        return new OpenAIRollbackGenerationService(httpClient, settings, NullLogger<OpenAIRollbackGenerationService>.Instance);
    }

    private static HttpResponseMessage CreateOpenAIResponse(string contentText)
    {
        var response = new OpenAIValidationResponse(
            Id: "id",
            Object: "response",
            CreatedAt: 1,
            Status: "completed",
            Output:
            [
                new OpenAIOutput(
                    Id: "out",
                    Type: "message",
                    Status: "completed",
                    Content:
                    [
                        new OpenAIContentOutput(Type: "output_text", Text: contentText)
                    ],
                    Role: "assistant")
            ],
            Model: "gpt-4",
            MaxOutputTokens: 600,
            Usage: new OpenAIUsage(1, 1, 2));

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }

    private sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        public HttpResponseMessage Response { get; set; } = new(HttpStatusCode.OK);
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(Response);
        }
    }
}