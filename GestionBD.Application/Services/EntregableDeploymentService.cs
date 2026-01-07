using GestionBD.Application.Abstractions;
using GestionBD.Application.Contracts.Entregables;
using GestionBD.Domain.ValueObjects;
using Microsoft.Data.SqlClient;

namespace GestionBD.Application.Services
{
    public sealed class EntregableDeploymentService
    {
        private readonly IEntregableReadRepository _entregableReadRepository;
        private readonly IArtefactoReadRepository _artefactoReadRepository;
        private readonly IScriptExecutor _scriptExecutor;

        public EntregableDeploymentService(
            IEntregableReadRepository entregableReadRepository,
            IArtefactoReadRepository artefactoReadRepository,
            IScriptExecutor scriptExecutor)
        {
            _entregableReadRepository = entregableReadRepository;
            _artefactoReadRepository = artefactoReadRepository;
            _scriptExecutor = scriptExecutor;
        }

        public async Task<IEnumerable<EntregablePreValidateResponse>> DeployAsync(
            decimal idEntregable,
            CancellationToken cancellationToken = default)
        {
            // 1. Obtener el entregable
            var entregable = await _entregableReadRepository.GetByIdAsync(idEntregable, cancellationToken);
            
            if (entregable == null)
                throw new InvalidOperationException($"No se encontró el entregable con ID {idEntregable}");

            if (string.IsNullOrWhiteSpace(entregable.TemporalBD))
                throw new InvalidOperationException($"El entregable {idEntregable} no tiene una base de datos temporal asignada");

            // 2. Obtener los artefactos
            var artefactos = await _artefactoReadRepository.GetByEntregableIdAsync(idEntregable, cancellationToken);
            
            // 3. Convertir artefactos a entidades del dominio
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

            // 4. Extraer scripts del ZIP usando lógica de dominio
            var scripts = ScriptDeployment.ExtractScriptsFromZip(entregable.RutaEntregable, artefactosEntidades);

            // 5. Ejecutar cada script
            var results = new List<EntregablePreValidateResponse>();

            foreach (var script in scripts)
            {
                var result = await ExecuteScriptAsync(script, entregable.TemporalBD, cancellationToken);
                results.Add(result);

                if (!result.IsValid)
                    break; // Detener en caso de error
            }

            return results;
        }

        private async Task<EntregablePreValidateResponse> ExecuteScriptAsync(
            ScriptDeployment script,
            string databaseName,
            CancellationToken cancellationToken)
        {
            try
            {
                var batches = script.SplitIntoBatches();

                foreach (var batch in batches)
                {
                    await _scriptExecutor.ExecuteAsync(databaseName, batch, cancellationToken);
                }

                return new EntregablePreValidateResponse(
                    IsValid: true,
                    Script: script.ScriptName,
                    Status: "Success",
                    Message: "Script ejecutado correctamente",
                    AdditionalInfo: $"Orden de ejecución: {script.ExecutionOrder}"
                );
            }
            catch (SqlException sqlEx)
            {
                return new EntregablePreValidateResponse(
                    IsValid: false,
                    Script: script.ScriptName,
                    Status: "SqlError",
                    Message: $"Error SQL: {sqlEx.Message}",
                    AdditionalInfo: $"Número de error: {sqlEx.Number}, Línea: {sqlEx.LineNumber}"
                );
            }
            catch (Exception ex)
            {
                return new EntregablePreValidateResponse(
                    IsValid: false,
                    Script: script.ScriptName,
                    Status: "Error",
                    Message: ex.Message,
                    AdditionalInfo: ex.StackTrace
                );
            }
        }
    }
}