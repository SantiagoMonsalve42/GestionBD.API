namespace GestionBD.Domain.Interfaces
{
    public interface IUploadDelivery
    {
        Task<bool> uploadFile(Stream zipFile, string fileName, CancellationToken cancellationToken);
    }
}
