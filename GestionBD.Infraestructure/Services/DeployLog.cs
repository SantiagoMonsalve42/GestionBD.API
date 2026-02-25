using GestionBD.Application.Abstractions.Services;
using GestionBD.Application.Contracts.Entregables;
using System.IO;
using System.Text;

namespace GestionBD.Infrastructure.Services;

public sealed class DeployLog : IDeployLog
{
    public async Task<string> GenerarArchivoLog(
        IEnumerable<EntregablePreValidateResponse> respuestaEjecuciones, 
        string ruta, 
        string nombreArchivo)
    {
        if (respuestaEjecuciones == null)
            throw new ArgumentNullException(nameof(respuestaEjecuciones));

        if (string.IsNullOrWhiteSpace(ruta))
            throw new ArgumentException("La ruta no puede estar vacía", nameof(ruta));

        if (string.IsNullOrWhiteSpace(nombreArchivo))
            throw new ArgumentException("El nombre del archivo no puede estar vacío", nameof(nombreArchivo));

        // Asegurar que el nombre tiene extensión .txt
        if (!nombreArchivo.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
        {
            nombreArchivo += ".txt";
        }
        ruta = Path.GetDirectoryName(ruta);
        
        if (!Directory.Exists(ruta))
        {
            Directory.CreateDirectory(ruta);
        }
        DeleteExistingFiles(ruta);
        var fullPath = Path.Combine(ruta, nombreArchivo);
        var logContent = GenerarContenidoLog(respuestaEjecuciones);

        await File.WriteAllTextAsync(fullPath, logContent, Encoding.UTF8);

        return fullPath;
    }

    private static string GenerarContenidoLog(IEnumerable<EntregablePreValidateResponse> respuestas)
    {
        var sb = new StringBuilder();
        var respuestasList = respuestas.ToList();

        // Encabezado del log
        sb.AppendLine("═══════════════════════════════════════════════════════════════════");
        sb.AppendLine("              REPORTE DE VALIDACIÓN Y EJECUCIÓN DE SCRIPTS         ");
        sb.AppendLine("═══════════════════════════════════════════════════════════════════");
        sb.AppendLine($"Fecha de generación: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"Total de scripts procesados: {respuestasList.Count}");
        sb.AppendLine();

        // Resumen ejecutivo
        var exitosos = respuestasList.Count(r => r.IsValid);
        var fallidos = respuestasList.Count(r => !r.IsValid);

        sb.AppendLine("─────────────────────────────────────────────────────────────────");
        sb.AppendLine("                         RESUMEN EJECUTIVO                        ");
        sb.AppendLine("─────────────────────────────────────────────────────────────────");
        sb.AppendLine($"Scripts exitosos:    {exitosos} ({(respuestasList.Count > 0 ? (exitosos * 100.0 / respuestasList.Count):0):F2}%)");
        sb.AppendLine($"Scripts fallidos:    {fallidos} ({(respuestasList.Count > 0 ? (fallidos * 100.0 / respuestasList.Count):0):F2}%)");
        sb.AppendLine();

        // Detalle de cada script
        sb.AppendLine("═══════════════════════════════════════════════════════════════════");
        sb.AppendLine("                      DETALLE DE EJECUCIONES                       ");
        sb.AppendLine("═══════════════════════════════════════════════════════════════════");
        sb.AppendLine();

        int contador = 1;
        foreach (var respuesta in respuestasList)
        {
            var estado = respuesta.IsValid ? "ÉXITO" : "FALLIDO";

            sb.AppendLine($"[{contador}] SCRIPT: {respuesta.Script}");
            sb.AppendLine(new string('─', 70));
            sb.AppendLine($"Estado:        {estado}");
            sb.AppendLine($"Status:        {respuesta.Status}");
            
            if (!string.IsNullOrWhiteSpace(respuesta.Message))
            {
                sb.AppendLine($"Mensaje:       {respuesta.Message}");
            }
            
            if (!string.IsNullOrWhiteSpace(respuesta.AdditionalInfo))
            {
                sb.AppendLine($"Info Adicional: {respuesta.AdditionalInfo}");
            }
            
            sb.AppendLine();
            contador++;
        }

        // Pie del reporte
        sb.AppendLine("═══════════════════════════════════════════════════════════════════");
        sb.AppendLine("                         FIN DEL REPORTE                           ");
        sb.AppendLine("═══════════════════════════════════════════════════════════════════");

        return sb.ToString();
    }
    private static void DeleteExistingFiles(string path)
    {
        foreach (var file in Directory.GetFiles(path, "*.txt"))
        {
            try
            {
                File.Delete(file);
            }
            catch (IOException ex)
            {
                // log o manejo de error
                Console.WriteLine($"No se pudo borrar {file}: {ex.Message}");
            }
        }
    }
}
