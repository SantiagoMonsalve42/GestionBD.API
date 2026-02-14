using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Abstractions.Services;
using GestionBD.Application.Contracts.Instancias;
using GestionBD.Application.Entregables.Commands;
using GestionBD.Domain;
using GestionBD.Domain.Services;
using GestionBD.Domain.ValueObjects;
using MediatR;
using System.Collections.Generic;
using System.Text;

namespace GestionBD.Application.Entregables.CommandsHandlers
{
    public class GenerateRollbackCommandHandler : IRequestHandler<GenerateRollbackCommand, string?>
    {
        private readonly IScriptRegexService _scriptRegexService;
        private readonly IEntregableReadRepository _entregableReadRepository;
        private readonly IArtefactoReadRepository _artefactoReadRepository;
        private readonly IDatabaseService _databaseService;
        private readonly IInstanciaReadRepository _instanciaReadRepository;
        private readonly IRollbackGenerationService _rollbackScriptGeneratorService;
        private readonly IRollbackService _rollbackService;
        private readonly IUnitOfWork _unitOfWork;
        public GenerateRollbackCommandHandler(IScriptRegexService scriptRegexService,
                                              IEntregableReadRepository entregableReadRepository,
                                              IArtefactoReadRepository artefactoReadRepository,
                                              IDatabaseService databaseService,
                                              IInstanciaReadRepository instanciaReadRepository,
                                              IRollbackGenerationService rollbackScriptGeneratorService,
                                              IRollbackService rollbackService,
                                              IUnitOfWork unitOfWork)
        {
            _scriptRegexService = scriptRegexService;
            _entregableReadRepository = entregableReadRepository;
            _artefactoReadRepository = artefactoReadRepository;
            _databaseService = databaseService;
            _rollbackScriptGeneratorService = rollbackScriptGeneratorService;
            _instanciaReadRepository = instanciaReadRepository;
            _rollbackService = rollbackService;
            _unitOfWork = unitOfWork;
        }
        public async Task<string?> Handle(GenerateRollbackCommand request, CancellationToken cancellationToken)
        {
            
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);
                var entregable = await _entregableReadRepository.GetByIdAsync(request.idEntregable,cancellationToken);

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

                var rollbackResponses = await getRollbackResponses(scripts, datosInstancia);
                var zipPath = await _rollbackService.GenerateRollbackScriptAsync(rollbackResponses, entregable.RutaEntregable, cancellationToken);

                await _unitOfWork.Entregables.UpdateEstado(request.idEntregable, Domain.Enum.EstadoEntregaEnum.Rollback, cancellationToken);
                await _unitOfWork.Entregables.UpdateRutaRollback(request.idEntregable, zipPath, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return string.IsNullOrEmpty(zipPath) ? "No se pudo generar el archivo" : $"Archivo de rollback generado en la ruta: {zipPath}";
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return $"Error al generar el archivo de rollback: {ex.Message}";
            }
        }
        private async Task<List<RollbackGeneration>> getRollbackResponses(IEnumerable<ScriptDeployment> scripts, InstanciaConnectResponse instanciaConnectResponse)
        {
            List<RollbackGeneration> tasksCompleted = new List<RollbackGeneration>();
            var tasksPending = scripts.Select(async script =>
                tasksCompleted.Add(await generateRollbackServiceScript(script.ScriptContent, await getRelatedObjects(script, instanciaConnectResponse)))
                );
            await Task.WhenAll(tasksPending);
            return tasksCompleted;
        }
        private async Task<string> getRelatedObjects(ScriptDeployment script, InstanciaConnectResponse instanciaConnectResponse)
        {
            var objectsInScript = _scriptRegexService.getRelatedObjects(script.ScriptContent);
            
            if (!objectsInScript.Any())
                return string.Empty;
            
            using var semaphore = new SemaphoreSlim(Environment.ProcessorCount, Environment.ProcessorCount);
            
            var tasks = objectsInScript.Select(async objectScript =>
            {
                await semaphore.WaitAsync();
                try
                {
                    return await _databaseService.getObjectDefinition(
                        $"{instanciaConnectResponse.Instancia},{instanciaConnectResponse.Puerto}",
                        instanciaConnectResponse.NombreBD,
                        instanciaConnectResponse.Usuario,
                        instanciaConnectResponse.Password, 
                        objectScript);
                }
                finally
                {
                    semaphore.Release();
                }
            });
            
            var relatedObjects = await Task.WhenAll(tasks);
            
            return scriptListToContext(relatedObjects.Where(x => !string.IsNullOrEmpty(x)).ToList());
        }
        private string scriptListToContext(List<string> objectDefinitions)
        {
            StringBuilder stringBuilder = new StringBuilder();
            int index = 0;
            foreach (var definition in objectDefinitions)
            {
                stringBuilder.AppendLine($"{index}. {definition} | ");
                index++;
            }
            return stringBuilder.ToString();    
        }
        private async Task<RollbackGeneration> generateRollbackServiceScript(string newObjectsDefinitions,
                                            string currentObjectsDefinitions)
        {
            return await _rollbackScriptGeneratorService
                            .GenerateRollbackAsync(newObjectsDefinitions, currentObjectsDefinitions);
        }
        
    }
}
