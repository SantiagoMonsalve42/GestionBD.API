namespace GestionBD.Application.Abstractions.Services
{
    public interface IScriptRegexService
    {
        public List<string> getRelatedObjects(string script);
    }
}
