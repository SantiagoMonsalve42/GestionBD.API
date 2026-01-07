namespace GestionBD.Application.Abstractions
{
    /// <summary>
    /// Ejecutor de scripts SQL (detalle técnico de infraestructura)
    /// </summary>
    public interface IScriptExecutor
    {
        Task ExecuteAsync(string databaseName, string scriptBatch, CancellationToken cancellationToken = default);
    }
}