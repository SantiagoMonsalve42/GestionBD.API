using GestionBD.API.Models;
using GestionBD.Domain.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Net;
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

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
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

        // Agregar detalles técnicos solo en desarrollo
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

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, options));
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