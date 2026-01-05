using GestionBD.Domain.Exceptions;
using System.IO.Compression;

namespace GestionBD.Domain.ValueObjects;

public sealed record ArchivoEntregable
{
    private const long MaxFileSize = 100 * 1024 * 1024; // 100 MB
    private static readonly string[] AllowedExtensions = { ".zip" };
    private static readonly string[] AllowedInnerExtensions = { ".sql" };

    public string FileName { get; }
    public long FileSize { get; }
    public string Extension { get; }

    private ArchivoEntregable(string fileName, long fileSize)
    {
        FileName = fileName;
        FileSize = fileSize;
        Extension = Path.GetExtension(fileName).ToLowerInvariant();
    }

    public static ArchivoEntregable Crear(string fileName, long fileSize)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ValidationException("FileName", "El nombre del archivo es requerido");
        }

        if (fileSize <= 0)
        {
            throw new ValidationException("FileSize", "El archivo está vacío");
        }

        if (fileSize > MaxFileSize)
        {
            throw new ValidationException(
                "FileSize", 
                $"El archivo excede el tamaño máximo permitido de {MaxFileSize / (1024 * 1024)} MB");
        }

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            throw new ValidationException(
                "FileName", 
                $"Solo se permiten archivos con extensiones: {string.Join(", ", AllowedExtensions)}");
        }

        return new ArchivoEntregable(fileName, fileSize);
    }

    /// <summary>
    /// Valida que todos los archivos dentro del .zip tengan extensión .sql
    /// </summary>
    public static void ValidarContenidoZip(Stream zipStream)
    {
        if (zipStream == null || !zipStream.CanRead)
        {
            throw new ValidationException("File", "El stream del archivo no es válido");
        }

        try
        {
            using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read, leaveOpen: true);

            if (archive.Entries.Count == 0)
            {
                throw new ValidationException("File", "El archivo .zip está vacío");
            }

            var archivosInvalidos = new List<string>();

            foreach (var entry in archive.Entries)
            {
                // Ignorar directorios (entries sin nombre de archivo)
                if (string.IsNullOrWhiteSpace(entry.Name))
                    continue;

                var entryExtension = Path.GetExtension(entry.Name).ToLowerInvariant();

                if (!AllowedInnerExtensions.Contains(entryExtension))
                {
                    archivosInvalidos.Add(entry.FullName);
                }
            }

            if (archivosInvalidos.Any())
            {
                throw new ValidationException(
                    "File",
                    $"El archivo .zip contiene archivos no permitidos. Solo se permiten archivos .sql. Archivos inválidos: {string.Join(", ", archivosInvalidos)}");
            }

            // Resetear el stream a la posición inicial para permitir su uso posterior
            if (zipStream.CanSeek)
            {
                zipStream.Position = 0;
            }
        }
        catch (InvalidDataException)
        {
            throw new ValidationException("File", "El archivo .zip está corrupto o no es un archivo válido");
        }
        catch (ValidationException)
        {
            // Re-lanzar las excepciones de validación
            throw;
        }
        catch (Exception ex)
        {
            throw new ValidationException("File", $"Error al validar el contenido del archivo .zip: {ex.Message}");
        }
    }
}