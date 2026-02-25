using GestionBD.Application.Configuration;
using GestionBD.Application.DTO.OpenAI;
using GestionBD.Infrastructure.ExternalServices.OpenAI;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using System.Text.Json;

namespace GestionBD.UnitTests.Infrastructure.ExternalServices.OpenAI;

public sealed class OpenAISqlValidationServiceTests
{
    [Fact]
    public void Constructor_WithMissingBaseUrl_ShouldThrowInvalidOperationException()
    {
        var settings = Options.Create(new OpenIASettings { ApiKey = "key", Model = "gpt-4", BaseURL = "" });
        var httpClient = new HttpClient(new FakeHttpMessageHandler());

        Assert.Throws<InvalidOperationException>(() =>
            new OpenAISqlValidationService(httpClient, settings, NullLogger<OpenAISqlValidationService>.Instance));
    }

    [Fact]
    public async Task ValidateScriptAsync_WithEmptyScript_ShouldThrowArgumentException()
    {
        var service = CreateService(out _);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.ValidateScriptAsync(false, string.Empty));

        Assert.Equal("sqlScript", exception.ParamName);
    }

    [Fact]
    public async Task ValidateScriptAsync_WithValidResponse_ShouldReturnValidation()
    {
        var service = CreateService(out var handler);

        var validationJson = JsonSerializer.Serialize(new
        {
            isValid = true,
            errors = new[] { new { code = "E1", message = "Error" } },
            warnings = new[] { new { code = "W1", message = "Warning" } },
            suggestions = new[] { new { code = "S1", message = "Suggestion" } }
        }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        handler.Response = CreateOpenAIResponse(validationJson);

        var result = await service.ValidateScriptAsync(false, "SELECT 1;");

        Assert.True(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Single(result.Warnings);
        Assert.Single(result.Suggestions);
        Assert.Equal("/v1/responses", handler.LastRequest?.RequestUri?.AbsolutePath);
    }

    private static OpenAISqlValidationService CreateService(out FakeHttpMessageHandler handler)
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

        return new OpenAISqlValidationService(httpClient, settings, NullLogger<OpenAISqlValidationService>.Instance);
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