namespace GestionBD.Application.Contracts.LogTransacciones;

public sealed record LogTransaccionResponse(
    decimal IdTransaccion,
    string NombreTransaccion,
    string EstadoTransaccion,
    string DescripcionTransaccion,
    DateTime FechaInicio,
    string? RespuestaTransaccion,
    DateTime? FechaFin,
    string UsuarioEjecucion
);