using GestionBD.Application.Abstractions;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using MediatR;

namespace GestionBD.Application.Entregables.Commands;

public sealed class DeleteEntregableCommandHandler : IRequestHandler<DeleteEntregableCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    public DeleteEntregableCommandHandler(IUnitOfWork unitOfWork,IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<Unit> Handle(DeleteEntregableCommand command, CancellationToken cancellationToken)
    {
        var entregable = await _unitOfWork.FindEntityAsync<TblEntregable>(command.IdEntregable, cancellationToken);
        
        await _fileStorageService.DeleteFileAsync(entregable.RutaEntregable);
        
        if (entregable == null)
            throw new KeyNotFoundException($"Entregable con ID {command.IdEntregable} no encontrado.");

        _unitOfWork.Entregables.Remove(entregable);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}