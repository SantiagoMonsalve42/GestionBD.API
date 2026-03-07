using GestionBD.API.Models;
using GestionBD.Application.Abstractions.Services;
using GestionBD.Domain.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace GestionBD.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context, ILoggerAuditService loggerAuditService)
    {
        var userId = GetUserIdFromAuthorizationHeader(context);

        decimal logId = await loggerAuditService.LogAudit(
            userId: userId,
            action: $"{context.Request.Method} {context.Request.Path}",
            description: $"{context.Request.Method} {context.Request.Path}");

        try
        {
            await _next(context);

            var successJson = JsonSerializer.Serialize(new
            {
                message = "Solicitud procesada correctamente."
            });

            var responseAudit = $"{context.Response.StatusCode} - {successJson}";
            await loggerAuditService.UpdateLogAudit(logId, responseAudit, "S");
        }
        catch (Exception ex)
        {
            var errorAudit = await HandleExceptionAsync(context, ex);
            await loggerAuditService.UpdateLogAudit(logId, errorAudit, "F");
        }
    }

    private static string GetUserIdFromAuthorizationHeader(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
        {
            return "Anonymous";
        }

        var authorizationValue = authorizationHeader.ToString();
        if (!authorizationValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return "Anonymous";
        }

        var token = authorizationValue["Bearer ".Length..].Trim();
        if (string.IsNullOrWhiteSpace(token))
        {
            return "Anonymous";
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
            {
                return "Anonymous";
            }

            var jwtToken = handler.ReadJwtToken(token);

            return jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value
                ?? "Anonymous";
        }
        catch
        {
            return "Anonymous";
        }
    }

    private async Task<string> HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;

        _logger.LogError(exception,
            "Error no controlado. TraceId: {TraceId}, Path: {Path}",
            traceId,
            context.Request.Path);

        var errorResponse = exception switch
        {
            SqlException sqlEx => HandleSqlException(sqlEx, traceId),

            ValidationException validationEx => CreateErrorResponse(
                HttpStatusCode.BadRequest,
                validationEx.Message,
                traceId,
                validationEx.Errors),

            NotFoundException notFoundEx => CreateErrorResponse(
                HttpStatusCode.NotFound,
                notFoundEx.Message,
                traceId),

            ConflictException conflictEx => CreateErrorResponse(
                HttpStatusCode.Conflict,
                conflictEx.Message,
                traceId),

            UnauthorizedException unauthorizedEx => CreateErrorResponse(
                HttpStatusCode.Unauthorized,
                unauthorizedEx.Message,
                traceId),

            ForbiddenException forbiddenEx => CreateErrorResponse(
                HttpStatusCode.Forbidden,
                forbiddenEx.Message,
                traceId),

            DbUpdateException dbUpdateEx => HandleDbUpdateException(dbUpdateEx, traceId),

            InvalidOperationException invalidOpEx => CreateErrorResponse(
                HttpStatusCode.BadRequest,
                invalidOpEx.Message,
                traceId),

            ArgumentException argEx => CreateErrorResponse(
                HttpStatusCode.BadRequest,
                argEx.Message,
                traceId),

            _ => CreateErrorResponse(
                HttpStatusCode.InternalServerError,
                "Ha ocurrido un error interno en el servidor.",
                traceId)
        };

        if (_environment.IsDevelopment() && errorResponse.StatusCode == 500)
        {
            errorResponse.Details = exception.ToString();
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = errorResponse.StatusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        };

        var serialized = JsonSerializer.Serialize(errorResponse, options);
        await context.Response.WriteAsync(serialized);

        return $"{errorResponse.StatusCode} - {serialized}";
    }

    private ErrorResponse CreateErrorResponse(
        HttpStatusCode statusCode,
        string message,
        string traceId,
        IDictionary<string, string[]>? errors = null)
    {
        return new ErrorResponse
        {
            StatusCode = (int)statusCode,
            Message = message,
            TraceId = traceId,
            Timestamp = DateTime.UtcNow,
            Errors = errors as Dictionary<string, string[]>
        };
    }

    private ErrorResponse HandleSqlException(SqlException sqlEx, string traceId)
    {
        var message = sqlEx.Number switch
        {
            -1 => "Error de conexión con la base de datos. Por favor, intente nuevamente.",
            -2 => "Tiempo de espera agotado al conectar con la base de datos.",
            2627 or 2601 => "Ya existe un registro con estos datos.",
            547 => "No se puede completar la operación debido a restricciones de integridad referencial.",
            _ => "Error al procesar la operación en la base de datos."
        };

        _logger.LogError(sqlEx, "Error SQL: {ErrorNumber} - {Message}", sqlEx.Number, sqlEx.Message);

        return CreateErrorResponse(HttpStatusCode.BadRequest, message, traceId);
    }

    private ErrorResponse HandleDbUpdateException(DbUpdateException dbUpdateEx, string traceId)
    {
        var message = "Error al actualizar la base de datos.";

        if (dbUpdateEx.InnerException is SqlException sqlEx)
        {
            return HandleSqlException(sqlEx, traceId);
        }

        _logger.LogError(dbUpdateEx, "Error al actualizar la base de datos");

        return CreateErrorResponse(HttpStatusCode.BadRequest, message, traceId);
    }
}