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

    public CreateEntregableWithFileCommandHandler(
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<decimal> Handle(CreateEntregableWithFileCommand command, CancellationToken cancellationToken)
    {
        var file = command.Request.File;

        // Validar el archivo usando el Value Object del dominio
        var archivoEntregable = ArchivoEntregable.Crear(file.FileName, file.Length);

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
            NumeroEntrega = command.Request.NumeroEntrega
        };

        _unitOfWork.Entregables.Add(entregable);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entregable.IdEntregable;
    }
}