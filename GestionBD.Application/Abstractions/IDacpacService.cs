namespace GestionBD.Application.Abstractions
{
    public interface IDacpacService
    {
        /// <summary>
        /// Extrae un DACPAC de una base de datos SQL Server y lo guarda en una ruta local
        /// </summary>
        /// <param name="serverName">Nombre o IP del servidor</param>
        /// <param name="databaseName">Nombre de la base de datos</param>
        /// <param name="username">Usuario de SQL Server (opcional si usa autenticación Windows)</param>
        /// <param name="password">Contraseña (opcional si usa autenticación Windows)</param>
        /// <param name="outputPath">Ruta donde se guardará el DACPAC (opcional)</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Ruta completa del archivo DACPAC generado</returns>
        Task<string> ExtractDacpacAsync(string serverName, 
                                        string databaseName, 
                                        string? username = null, 
                                        string? password = null, 
                                        CancellationToken cancellationToken = default);
    }
}