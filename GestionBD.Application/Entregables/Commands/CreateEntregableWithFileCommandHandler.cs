using MediatR;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using GestionBD.Domain.ValueObjects;
using GestionBD.Application.Abstractions;
using GestionBD.Domain.Exceptions;

namespace GestionBD.Application.Entregables.Commands;

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
        int cantidadEntregas = (await _entregableReadRepository.GetEntregablesByEjecucion(command.Request.IdEjecucion)) + 1;
        var ejecucion = await _ejecucionReadRepository.GetByIdAsync(command.Request.IdEjecucion, cancellationToken);
        // Validar el archivo usando el Value Object del dominio
        var archivoEntregable = ArchivoEntregable.Crear(file.FileName, file.Length,ejecucion.NombreRequerimiento,cantidadEntregas);

        // Abrir el stream del archivo
        using var stream = file.OpenReadStream();

        // Validar que todos los archivos dentro del .zip sean .sql
        ArchivoEntregable.ValidarContenidoZip(stream);

        // Guardar el archivo y obtener la ruta
        string rutaEntregable;
        try
        {
            rutaEntregable = await _fileStorageService.SaveFileAsync(
                stream,
                archivoEntregable.FileName,
                cancellationToken);
        }
        catch (Exception ex)
        {
            throw new ValidationException("File", $"Error al guardar el archivo: {ex.Message}");
        }

        // Crear el entregable con la ruta del archivo guardado
        

        var entregable = new TblEntregable
        {
            RutaEntregable = rutaEntregable,
            DescripcionEntregable = command.Request.DescripcionEntregable,
            IdEjecucion = command.Request.IdEjecucion,
            NumeroEntrega = cantidadEntregas
        };

        _unitOfWork.Entregables.Add(entregable);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entregable.IdEntregable;
    }
}