namespace GestionBD.Application.Abstractions.Services
{
    public interface IDatabaseService
    {
        public Task<string> getObjectDefinition(string serverName,
                                                string databaseName,
                                                string user,
                                                string password,
                                                string objectName);
    }
}
