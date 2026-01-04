using GestionBD.Domain.Interfaces;
using System.Threading;

namespace GestionBD.Domain.Implementations
{
    public class UploadDelivery : IUploadDelivery
    {
        private string route;
        public UploadDelivery(string route) { 
            this.route = route;
        }

        public async Task<bool> uploadFile(Stream zipFile, string fileName, CancellationToken cancellationToken)
        {
            if (zipFile == null)
                throw new ArgumentNullException(nameof(zipFile));

            if (string.IsNullOrWhiteSpace(this.route))
                throw new ArgumentException("Ruta destino inválida", nameof(this.route));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Nombre de archivo inválido", nameof(fileName));

            Directory.CreateDirectory(this.route);

            var rutaCompleta = Path.Combine(this.route, fileName);

            await using var fileStream = new FileStream(
                rutaCompleta,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920,
                useAsync: true);

            await zipFile.CopyToAsync(fileStream, cancellationToken);
            return true;
        }
    }
}
