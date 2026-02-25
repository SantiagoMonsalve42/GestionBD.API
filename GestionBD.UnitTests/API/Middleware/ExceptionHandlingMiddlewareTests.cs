using GestionBD.API.Middleware;
using GestionBD.API.Models;
using GestionBD.Domain.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GestionBD.UnitTests.API.Middleware;

public sealed class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WhenInvalidOperationException_ShouldReturnBadRequest()
    {
        var context = new DefaultHttpContext
        {
            TraceIdentifier = "trace-1"
        };
        context.Response.Body = new MemoryStream();

        RequestDelegate next = _ => throw new InvalidOperationException("Invalid operation");
        var logger = LoggerFactory.Create(builder => { }).CreateLogger<ExceptionHandlingMiddleware>();
        var environment = new TestWebHostEnvironment(Environments.Production);
        var middleware = new ExceptionHandlingMiddleware(next, logger, environment);

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.NotNull(response);
        Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        Assert.Equal("Invalid operation", response.Message);
        Assert.Equal("trace-1", response.TraceId);
    }

    [Fact]
    public async Task InvokeAsync_WhenNotFoundException_ShouldReturnNotFound()
    {
        var context = new DefaultHttpContext
        {
            TraceIdentifier = "trace-2"
        };
        context.Response.Body = new MemoryStream();

        RequestDelegate next = _ => throw new NotFoundException("Entidad", 1);
        var logger = LoggerFactory.Create(builder => { }).CreateLogger<ExceptionHandlingMiddleware>();
        var environment = new TestWebHostEnvironment(Environments.Production);
        var middleware = new ExceptionHandlingMiddleware(next, logger, environment);

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var response = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
        Assert.NotNull(response);
        Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
        Assert.Contains("no fue encontrada", response.Message, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class TestWebHostEnvironment : IWebHostEnvironment
    {
        public TestWebHostEnvironment(string environmentName)
        {
            EnvironmentName = environmentName;
            ApplicationName = "GestionBD.Tests";
            ContentRootPath = Directory.GetCurrentDirectory();
            WebRootPath = ContentRootPath;
            ContentRootFileProvider = new NullFileProvider();
            WebRootFileProvider = new NullFileProvider();
        }

        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; }
        public string WebRootPath { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
        public string ContentRootPath { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
    }
}