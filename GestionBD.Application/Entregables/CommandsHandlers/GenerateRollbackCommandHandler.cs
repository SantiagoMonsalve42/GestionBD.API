using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Abstractions.Services;
using GestionBD.Application.Contracts.Instancias;
using GestionBD.Application.Entregables.Commands;
using GestionBD.Domain.ValueObjects;
using MediatR;
using System.Collections.Generic;

namespace GestionBD.Application.Entregables.CommandsHandlers
{
    public class GenerateRollbackCommandHandler : IRequestHandler<GenerateRollbackCommand, string>
    {
        private readonly IScriptRegexService _scriptRegexService;
        private readonly IEntregableReadRepository _entregableReadRepository;
        private readonly IArtefactoReadRepository _artefactoReadRepository;
        private readonly IDatabaseService _databaseService;
        private readonly IInstanciaReadRepository _instanciaReadRepository;
        public GenerateRollbackCommandHandler(IScriptRegexService scriptRegexService,
                                              IEntregableReadRepository entregableReadRepository,
                                              IArtefactoReadRepository artefactoReadRepository,
                                              IDatabaseService databaseService,
                                              IInstanciaReadRepository instanciaReadRepository)
        {
            _scriptRegexService = scriptRegexService;
            _entregableReadRepository = entregableReadRepository;
            _artefactoReadRepository = artefactoReadRepository;
            _databaseService = databaseService;
            _instanciaReadRepository = instanciaReadRepository;
        }
        public async Task<string> Handle(GenerateRollbackCommand request, CancellationToken cancellationToken)
        {
            var entregable = await _entregableReadRepository.GetByIdAsync(
            request.idEntregable,
            cancellationToken);

            if (entregable == null)
                throw new InvalidOperationException(
                    $"No se encontró el entregable con ID {request.idEntregable}");
            // 1.1 Obtener detalles de conexión
            var datosInstancia = await _instanciaReadRepository.GetConnectionDetailsByEntregableIdAsync(entregable.IdEntregable, cancellationToken);

            if (datosInstancia == null)
                throw new InvalidOperationException($"No se encontró conexion parametrizada con ID {request.idEntregable}");

            var artefactos = await _artefactoReadRepository.GetByEntregableIdAsync(
                request.idEntregable,
                cancellationToken);

            var artefactosEntidades = artefactos.Select(a => new Domain.Entities.TblArtefacto
            {
                IdArtefacto = a.IdArtefacto,
                IdEntregable = a.IdEntregable,
                OrdenEjecucion = a.OrdenEjecucion,
                Codificacion = a.Codificacion,
                NombreArtefacto = a.NombreArtefacto,
                RutaRelativa = a.RutaRelativa,
                EsReverso = a.EsReverso
            }).ToList();

            var scripts = ScriptDeployment.ExtractScriptsFromZip(
                entregable.RutaEntregable,
                artefactosEntidades);
            List<string> relatedObject = getRelatedObjects(scripts);
            List<string> objectDefinitions = await getObjectDefinitions(relatedObject, datosInstancia);
            return null;
        }
        private List<string> getRelatedObjects(IEnumerable<ScriptDeployment> scripts)
        {
            var relatedObjects = new List<string>();
            foreach (var script in scripts)
            {
                var objectsInScript = _scriptRegexService.getRelatedObjects(script.ScriptContent);
                relatedObjects.AddRange(objectsInScript);
            }
            return relatedObjects.Distinct().ToList();
        }
        private async Task<List<string>> getObjectDefinitions(List<string> scripts, InstanciaConnectResponse instanciaConnectResponse)
        {
            var objectDefinitions = new List<string>();
            foreach (var script in scripts)
            {
                string definition = await _databaseService.getObjectDefinition($"{instanciaConnectResponse.Instancia},{instanciaConnectResponse.Puerto}",
                                                    instanciaConnectResponse.NombreBD,
                                                    instanciaConnectResponse.Usuario,
                                                    instanciaConnectResponse.Password,
                                                    script);
                objectDefinitions.Add(definition);
            }
            return objectDefinitions.Where(x => !string.IsNullOrEmpty(x)).ToList();
        }
    }
}
