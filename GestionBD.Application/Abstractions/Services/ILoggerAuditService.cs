namespace GestionBD.Application.Abstractions.Services
{
    public interface ILoggerAuditService
    {
        public Task<decimal> LogAudit(string userId, string action, string description);
        public Task UpdateLogAudit(decimal logId, string response,string status);
    }
}
