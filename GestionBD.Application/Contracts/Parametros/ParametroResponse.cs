namespace GestionBD.Application.Contracts.Parametros;

public sealed record ParametroResponse(
    decimal IdParametro,
    string NombreParametro,
    decimal? ValorNumerico,
    string? ValorString
);