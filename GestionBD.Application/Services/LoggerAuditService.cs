using GestionBD.Application.Abstractions.Services;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using System;

namespace GestionBD.Application.Services
{
    public class LoggerAuditService : ILoggerAuditService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LoggerAuditService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<decimal> LogAudit(string userId, string action, string description)
        {
            var transacc = new TblLogTransaccione
            {
                DescripcionTransaccion = description,
                EstadoTransaccion = "P",
                FechaFin = null,
                FechaInicio = DateTime.UtcNow,
                NombreTransaccion = action,
                RespuestaTransaccion = null,
                UsuarioEjecucion = userId
            };

            _unitOfWork.LogTransacciones.Add(transacc);
            await _unitOfWork.SaveChangesAsync();

            return transacc.IdTransaccion;
        }

        public async Task UpdateLogAudit(decimal logId, string response, string status)
        {
            var transacc = await _unitOfWork.FindEntityAsync<TblLogTransaccione>(logId);

            if (transacc is null)
            {
                throw new InvalidOperationException($"No se encontró el log de auditoría con Id {logId}.");
            }

            transacc.EstadoTransaccion = status;
            transacc.FechaFin = DateTime.UtcNow;
            transacc.RespuestaTransaccion = response;

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
