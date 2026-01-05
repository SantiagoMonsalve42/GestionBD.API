namespace GestionBD.Application.Abstractions;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);
    Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default);
}