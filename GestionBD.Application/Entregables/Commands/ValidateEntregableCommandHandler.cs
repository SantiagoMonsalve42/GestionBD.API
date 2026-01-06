using GestionBD.Application.Abstractions;
using GestionBD.Domain;
using MediatR;

namespace GestionBD.Application.Entregables.Commands
{
    public sealed class ValidateEntregableCommandHandler : IRequestHandler<ValidateEntregableCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInstanciaReadRepository _instanciaReadRepository;

        public ValidateEntregableCommandHandler(IUnitOfWork unitOfWork,
                                                IInstanciaReadRepository instanciaReadRepository)
        {
            _unitOfWork = unitOfWork;
            _instanciaReadRepository = instanciaReadRepository;
        }
        public async Task<string> Handle(ValidateEntregableCommand request, CancellationToken cancellationToken)
        {
            var entregable = await _instanciaReadRepository.GetConnectionDetailsByEntregableIdAsync(request.idEntregable);
            
            return "sisas";
        }
    }
}
