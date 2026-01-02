namespace GestionBD.Application.Contracts.Ejecuciones;

public sealed record EjecucionResponse(
    decimal IdEjecucion,
    decimal IdInstancia,
    DateTime HoraInicioEjecucion,
    DateTime HoraFinEjecucion,
    string? Descripcion,
    string? NombreInstancia
);