using GestionBD.Application.Configuration;
using GestionBD.Application.DTO.OpenAI;
using GestionBD.Domain.Services;
using GestionBD.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GestionBD.Infrastructure.ExternalServices.OpenAI;

public sealed class OpenAIRollbackGenerationService : IRollbackGenerationService
{
    private readonly HttpClient _httpClient;
    private readonly OpenIASettings _settings;
    private readonly ILogger<OpenAIRollbackGenerationService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    public OpenAIRollbackGenerationService(
        HttpClient httpClient,
        IOptions<OpenIASettings> settings,
        ILogger<OpenAIRollbackGenerationService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        ConfigureHttpClient();
    }

    public async Task<RollbackGeneration> GenerateRollbackAsync(
        string newObjectsDefinitions,
        string currentObjectsDefinitions,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(newObjectsDefinitions))
            throw new ArgumentException("Las definiciones nuevas no pueden estar vacías", nameof(newObjectsDefinitions));

        try
        {
            var request = BuildRollbackRequest(newObjectsDefinitions, currentObjectsDefinitions);
            _logger.LogInformation(
                "Enviando definiciones a OpenAI para generar rollback. Nuevas: {NewLength} caracteres, Actuales: {CurrentLength} caracteres",
                newObjectsDefinitions.Length, currentObjectsDefinitions.Length);

            var response = await _httpClient.PostAsJsonAsync(
                "/v1/responses",
                request,
                JsonOptions,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var openAIResponse = await response.Content.ReadFromJsonAsync<OpenAIValidationResponse>(
                JsonOptions,
                cancellationToken);

            if (openAIResponse == null)
                throw new InvalidOperationException("La respuesta de OpenAI es nula");

            var result = ParseToRollbackGeneration(openAIResponse);

            _logger.LogInformation(
                "Generación de rollback completada. Scripts: {Scripts}, Warnings: {Warnings}, Assumptions: {Assumptions}",
                result.ScriptCount, result.Warnings.Count, result.Assumptions.Count);

            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de comunicación con OpenAI API para rollback");
            throw new InvalidOperationException("Error al comunicarse con OpenAI para rollback", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error al deserializar la respuesta de rollback de OpenAI");
            throw new InvalidOperationException("Error al procesar la respuesta de rollback de OpenAI", ex);
        }
    }
    #region Private methods
    private void ConfigureHttpClient()
    {
        if (string.IsNullOrWhiteSpace(_settings.BaseURL))
            throw new InvalidOperationException("OpenAI BaseURL no está configurada");

        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            throw new InvalidOperationException("OpenAI ApiKey no está configurada");

        if (string.IsNullOrWhiteSpace(_settings.Model))
            throw new InvalidOperationException("OpenAI Model no está configurada");

        _httpClient.BaseAddress = new Uri(_settings.BaseURL);
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
        _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "responses=v1");
        _httpClient.Timeout = TimeSpan.FromMinutes(2); // Rollback puede tardar más
    }

    private RollbackGenerationRequest BuildRollbackRequest(
        string newObjectsDefinitions,
        string currentObjectsDefinitions)
    {
        var model = _settings.Model;
        var maxTokens = 600; // Más tokens para rollback

        var systemPrompt = @"Actúa como un arquitecto de base de datos senior y release engineer especializado en SQL Server 2022.

                            Objetivo: generar scripts de ROLLBACK que restauren exactamente el estado previo al despliegue.

                            Contexto de entrada:
                            - Definición NUEVA de los objetos a desplegar.
                            - Definición ACTUAL (pre-deploy) de los objetos relacionados.
                            zas
                            Reglas:
                            - Genera solo rollback.
                            - Restaura la versión previa de cada objeto.
                            - Respeta dependencias (FK, vistas, SP, funciones).
                            - SQL compatible con SQL Server 2022.
                            - No inventes objetos.
                            - No incluyas texto fuera del JSON.

                            Responde únicamente usando esta interfaz JSON:

                            {
                              ""rollbackScripts"": [
                                {
                                  ""fileName"": ""string"",
                                  ""objectType"": ""TABLE | VIEW | PROCEDURE | FUNCTION | INDEX | CONSTRAINT"",
                                  ""objectName"": ""string"",
                                  ""rollbackOrder"": number,
                                  ""script"": ""string"",
                                  ""dependsOn"": [""string""]
                                }
                              ]
                            }

                            Criterios:
                            - Un rollbackScript = un archivo .txt/.sql.
                            - Scripts completos y ejecutables.
                            - rollbackOrder define el orden correcto.
                            No incluyas reasoning, análisis, pensamientos internos ni explicaciones.
                            No incluyas campos distintos a los definidos en la interfaz JSON.
                            Genera el rollback basándote estrictamente en el contexto proporcionado.";

        var userPrompt = $@"DEFINICIONES NUEVAS (a desplegar):
                            {newObjectsDefinitions}

                            DEFINICIONES ACTUALES (pre-deploy):
                            {((string.IsNullOrEmpty(currentObjectsDefinitions))
? "No existen"
: currentObjectsDefinitions)}

                            Genera los scripts de rollback necesarios para restaurar el estado previo al despliegue.";

        return new RollbackGenerationRequest(
            Model: model,
            Input:
            [
                new OpenAIMessage(
                    Role: "system",
                    Content:
                    [
                        new OpenAIContent(Type: "input_text", Text: systemPrompt)
                    ]
                ),
                new OpenAIMessage(
                    Role: "user",
                    Content:
                    [
                        new OpenAIContent(Type: "input_text", Text: userPrompt)
                    ]
                )
            ],
            Text: new OpenAITextFormat(
                Format: new OpenAIFormatType(Type: "json_object")
            ),
            Reasoning: new OpenAIReasoning(Effort: "minimal"), // Más esfuerzo para rollback
            MaxOutputTokens: maxTokens
        );
    }

    private static RollbackGeneration ParseToRollbackGeneration(OpenAIValidationResponse response)
    {
        var messageOutput = response.Output
            .FirstOrDefault(o => o.Type == "message" && o.Content != null);

        if (messageOutput?.Content == null || messageOutput.Content.Count == 0)
        {
            throw new InvalidOperationException(
                "No se encontró contenido de mensaje en la respuesta de OpenAI para rollback");
        }

        var textContent = messageOutput.Content.First().Text;

        var rollbackDto = JsonSerializer.Deserialize<RollbackDto>(
            textContent,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        if (rollbackDto == null)
            throw new InvalidOperationException("No se pudo deserializar el resultado de rollback");


        var scripts = rollbackDto.RollbackScripts?.Select(s => new GestionBD.Domain.ValueObjects.RollbackScript(
            s.FileName,
            s.ObjectType,
            s.ObjectName,
            s.RollbackOrder,
            s.Script,
            s.DependsOn?.AsReadOnly() ?? new List<string>().AsReadOnly()
        )).ToList() ?? new List<GestionBD.Domain.ValueObjects.RollbackScript>();

        return RollbackGeneration.Create(
            scripts,
            rollbackDto.Warnings ?? [],
            rollbackDto.Assumptions ?? []
        );
    }


    private sealed record RollbackDto(
        MetadataDto Metadata,
        List<RollbackScriptDto>? RollbackScripts,
        List<string>? Warnings,
        List<string>? Assumptions
    );

    private sealed record MetadataDto(
        string Engine,
        string RollbackStrategy,
        string GeneratedAt
    );

    private sealed record RollbackScriptDto(
        string FileName,
        string ObjectType,
        string ObjectName,
        int RollbackOrder,
        string Script,
        List<string>? DependsOn
    );
    #endregion
}