using GestionBD.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace GestionBD.Infraestructure.Services;

public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public LocalFileStorageService(IConfiguration configuration)
    {
        _basePath = configuration["FileStorage:BasePath"] 
            ?? throw new InvalidOperationException("FileStorage:BasePath no está configurado");
        
        // Crear el directorio si no existe
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, fileName);
        string directoryPath = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        // Guardar el archivo
        using var fileStreamOutput = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await fileStream.CopyToAsync(fileStreamOutput, cancellationToken);

        return fullPath;
    }

    public Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(File.Exists(filePath));
    }
}