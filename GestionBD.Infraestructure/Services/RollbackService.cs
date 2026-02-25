using GestionBD.Application.Abstractions.Services;
using GestionBD.Domain.ValueObjects;
using System.IO.Compression;
using System.Text;

namespace GestionBD.Infrastructure.Services
{
    public class RollbackService : IRollbackService
    {
        public async Task<string> GenerateRollbackScriptAsync(List<RollbackGeneration> rollbackGenerations, 
                                                        string? currentPath, CancellationToken cancellationToken)
        {
            if (rollbackGenerations == null || !rollbackGenerations.Any())
                throw new ArgumentException("RollbackGenerations cannot be null or empty", nameof(rollbackGenerations));

            currentPath = Path.GetDirectoryName(currentPath);

            if (string.IsNullOrWhiteSpace(currentPath))
                throw new ArgumentException("CurrentPath cannot be null or empty", nameof(currentPath));


            var zipFilePath = Path.Combine(currentPath, "rollback.zip");

            if (File.Exists(zipFilePath))
                File.Delete(zipFilePath);

            using var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create);

            var allScripts = new List<RollbackScript>();

            foreach (var rollbackGeneration in rollbackGenerations)
            {
                allScripts.AddRange(rollbackGeneration.Scripts);
            }

            foreach (var script in allScripts)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var entry = zipArchive.CreateEntry(script.FileName, CompressionLevel.Optimal);
                
                using var entryStream = entry.Open();
                using var writer = new StreamWriter(entryStream, Encoding.UTF8);
                await writer.WriteAsync(script.Script);
            }
            return zipFilePath;
        }
    }
}
