using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Artefactos.Commands;
using GestionBD.Application.Contracts.Artefactos;
using GestionBD.Domain;
using GestionBD.Domain.Services;
using GestionBD.Domain.ValueObjects;
using MediatR;
using System.Collections.Concurrent;
using System.Text;

namespace GestionBD.Application.Artefactos.CommandsHandlers;

public sealed class ValidateArtefactoCommandHandler
    : IRequestHandler<ValidateArtefactoCommand, IEnumerable<ValidateArtefactoResponse>>
{
    private readonly IEntregableReadRepository _entregableReadRepository;
    private readonly IArtefactoReadRepository _artefactoReadRepository;
    private readonly ISqlValidationService _sqlValidationService;
    private readonly IUnitOfWork _unitOfWork;
    private const int MaxConcurrentValidations = 5; // Limitar a 5 validaciones simultáneas

    public ValidateArtefactoCommandHandler(
        IEntregableReadRepository entregableReadRepository,
        IArtefactoReadRepository artefactoReadRepository,
        ISqlValidationService sqlValidationService,
        IUnitOfWork unitOfWork)
    {
        _entregableReadRepository = entregableReadRepository
            ?? throw new ArgumentNullException(nameof(entregableReadRepository));
        _artefactoReadRepository = artefactoReadRepository
            ?? throw new ArgumentNullException(nameof(artefactoReadRepository));
        _sqlValidationService = sqlValidationService
            ?? throw new ArgumentNullException(nameof(sqlValidationService));
        _unitOfWork = unitOfWork
            ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<IEnumerable<ValidateArtefactoResponse>> Handle(
        ValidateArtefactoCommand request,
        CancellationToken cancellationToken)
    {
        var entregable = await _entregableReadRepository.GetByIdAsync(
            request.idEntregable,
            cancellationToken);

        if (entregable == null)
            throw new InvalidOperationException(
                $"No se encontró el entregable con ID {request.idEntregable}");

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

        var results = await ValidateScriptsWithThrottlingAsync(scripts, cancellationToken);
        var secuencialResult = await ValidateSecuencialExecution(scripts, cancellationToken);
        if (secuencialResult != null)
            results = results.Append(secuencialResult);
        await _unitOfWork.Entregables.UpdateEstado(request.idEntregable, Domain.Enum.EstadoEntregaEnum.Analisis, cancellationToken);
        await _unitOfWork.CommitTransactionAsync();
        return results;
    }
    private async Task<ValidateArtefactoResponse> ValidateSecuencialExecution(
        IEnumerable<ScriptDeployment> scripts,
        CancellationToken cancellationToken)
    {
        // Validate Order
        StringBuilder scriptSecuencialContent = new StringBuilder();
        int i = 1;
        foreach (var script in scripts)
        {
            scriptSecuencialContent.Append($"{i}. {script.ScriptContent} ");
            i++;
        }
        var validation = await _sqlValidationService.ValidateScriptAsync(
                    true,
                    scriptSecuencialContent.ToString(),
                    cancellationToken);
        return new ValidateArtefactoResponse(new string("Secuencial"), validation);
    }
    private async Task<IEnumerable<ValidateArtefactoResponse>> ValidateScriptsWithThrottlingAsync(
        IEnumerable<ScriptDeployment> scripts,
        CancellationToken cancellationToken)
    {
        using var semaphore = new SemaphoreSlim(MaxConcurrentValidations);
        var results = new ConcurrentBag<ValidateArtefactoResponse>();

        var validationTasks = scripts.Select(async script =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                var validation = await _sqlValidationService.ValidateScriptAsync(
                    false,
                    script.ScriptContent,
                    cancellationToken);

                results.Add(new ValidateArtefactoResponse(script.ScriptName, validation));
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(validationTasks);

        return results.OrderBy(r => r.Name);
    }
}
