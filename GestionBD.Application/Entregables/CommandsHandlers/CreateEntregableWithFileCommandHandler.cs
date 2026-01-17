using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Abstractions.Services;
using GestionBD.Application.Entregables.Commands;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using GestionBD.Domain.Enum;
using GestionBD.Domain.Exceptions;
using GestionBD.Domain.ValueObjects;
using MediatR;

namespace GestionBD.Application.Entregables.CommandsHandlers;

public sealed class CreateEntregableWithFileCommandHandler : IRequestHandler<CreateEntregableWithFileCommand, decimal>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly IEntregableReadRepository _entregableReadRepository;
    private readonly IEjecucionReadRepository _ejecucionReadRepository;

    public CreateEntregableWithFileCommandHandler(
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService,
        IEntregableReadRepository entregableReadRepository,
        IEjecucionReadRepository ejecucionReadRepository)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _entregableReadRepository = entregableReadRepository;
        _ejecucionReadRepository = ejecucionReadRepository;
    }

    public async Task<decimal> Handle(CreateEntregableWithFileCommand command, CancellationToken cancellationToken)
    {
        var file = command.Request.File;
        string? rutaEntregable = null;

        try
        {
            int cantidadEntregas = await _entregableReadRepository.GetEntregablesByEjecucion(command.Request.IdEjecucion) + 1;
            var ejecucion = await _ejecucionReadRepository.GetByIdAsync(command.Request.IdEjecucion, cancellationToken);
            
            if (ejecucion == null)
            {
                throw new ValidationException("IdEjecucion", "La ejecución no existe");
            }
            var entregableExistente = await _entregableReadRepository.GetAllByIdEjecucionAsync(
                command.Request.IdEjecucion,
                cancellationToken);
            

            if (entregableExistente.Any(x=> x.IdEjecucion != (int)EstadoEntregaEnum.Cerrado))
            {

                throw new ValidationException("EntregableExistente", "Ya existe un entregable abierto, primero cierrelo antes de crear otro.");
            }

            var archivoEntregable = ArchivoEntregable.Crear(
                file.FileName, 
                file.Length, 
                ejecucion.NombreRequerimiento, 
                cantidadEntregas);

            using var stream = file.OpenReadStream();

            ArchivoEntregable.ValidarContenidoZip(stream);

            rutaEntregable = await _fileStorageService.SaveFileAsync(
                stream,
                archivoEntregable.FileName,
                cancellationToken);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var entregable = new TblEntregable
                {
                    RutaEntregable = rutaEntregable,
                    DescripcionEntregable = command.Request.DescripcionEntregable,
                    IdEjecucion = command.Request.IdEjecucion,
                    NumeroEntrega = cantidadEntregas,
                    IdEstado = (int)EstadoEntregaEnum.Creado
                };

                _unitOfWork.Entregables.Add(entregable);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                var artefactos = ArchivoEntregable.ObtenerArtefactos(
                    stream, 
                    entregable.IdEntregable, 
                    rutaEntregable);

                foreach (var artefacto in artefactos)
                {
                    artefacto.IdEntregable = entregable.IdEntregable;
                    _unitOfWork.Artefactos.Add(artefacto);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return entregable.IdEntregable;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                if (!string.IsNullOrEmpty(rutaEntregable))
                {
                    await _fileStorageService.DeleteFileAsync(rutaEntregable, cancellationToken);
                }
                throw; 
            }
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrEmpty(rutaEntregable))
            {
                await _fileStorageService.DeleteFileAsync(rutaEntregable, cancellationToken);
            }

            throw new ValidationException("File", $"Error al procesar el entregable: {ex.Message}");
        }
    }
}