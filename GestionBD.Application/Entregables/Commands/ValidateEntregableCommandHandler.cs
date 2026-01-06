using GestionBD.Application.Abstractions;
using GestionBD.Domain;
using MediatR;

namespace GestionBD.Application.Entregables.Commands
{
    public sealed class ValidateEntregableCommandHandler : IRequestHandler<ValidateEntregableCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInstanciaReadRepository _instanciaReadRepository;
        private readonly IDacpacService _dacpacService;

        public ValidateEntregableCommandHandler(IUnitOfWork unitOfWork,
                                                IInstanciaReadRepository instanciaReadRepository,
                                                IDacpacService dacpacService)
        {
            _unitOfWork = unitOfWork;
            _instanciaReadRepository = instanciaReadRepository;
            _dacpacService = dacpacService;
        }

        public async Task<string> Handle(ValidateEntregableCommand request, CancellationToken cancellationToken)
        {
            var entregable = await _instanciaReadRepository.GetConnectionDetailsByEntregableIdAsync(request.idEntregable, cancellationToken);
            
            if (entregable == null)
                throw new InvalidOperationException($"No se encontraron detalles de conexión para el entregable {request.idEntregable}");

            // Extraer el DACPAC usando los detalles de conexión
            var dacpacPath = await _dacpacService.ExtractDacpacAsync(
                serverName: $"{entregable.Instancia},{entregable.Puerto}",
                databaseName: entregable.NombreBD,
                username: entregable.Usuario,
                password: entregable.Password,
                cancellationToken: cancellationToken
            );
            if(dacpacPath == null)
                throw new InvalidOperationException($"No se pudo extraer el DACPAC para el entregable {request.idEntregable}");
            
            await _unitOfWork.Entregables.UpdateDACPAC(request.idEntregable, dacpacPath);

            return dacpacPath;
        }
    }
}
