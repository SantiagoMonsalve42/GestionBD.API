namespace GestionBD.Application.Abstractions.Services
{
    /// <summary>
    /// Ejecutor de scripts SQL (detalle técnico de infraestructura)
    /// </summary>
    public interface IScriptExecutor
    {
        Task ExecuteAsync(
            string databaseName, 
            string scriptBatch, 
            string? serverName = null,
            string? username = null,
            string? password = null,
            CancellationToken cancellationToken = default);
    }
}