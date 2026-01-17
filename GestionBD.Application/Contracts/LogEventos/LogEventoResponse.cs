namespace GestionBD.Application.Contracts.LogEventos;

public sealed record LogEventoResponse(
    decimal IdEvento,
    decimal IdTransaccion,
    DateTime FechaEjecucion,
    string Descripcion,
    string EstadoEvento,
    string? NombreTransaccion
);