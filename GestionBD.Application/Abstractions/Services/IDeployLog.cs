using GestionBD.Application.Contracts.Entregables;

namespace GestionBD.Application.Abstractions.Services
{
    public interface IDeployLog
    {
        Task<string> GenerarArchivoLog(IEnumerable<EntregablePreValidateResponse> respuestaEjecuciones, string ruta, string nombreArchivo);


    }
}
