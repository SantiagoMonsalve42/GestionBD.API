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

            // Paso 1: Extraer el DACPAC de la BD origen
            var dacpacPath = await _dacpacService.ExtractDacpacAsync(
                serverName: $"{entregable.Instancia},{entregable.Puerto}",
                databaseName: entregable.NombreBD,
                username: entregable.Usuario,
                password: entregable.Password,
                cancellationToken: cancellationToken
            );

            if (string.IsNullOrWhiteSpace(dacpacPath))
                throw new InvalidOperationException($"No se pudo extraer el DACPAC para el entregable {request.idEntregable}");
            
            await _unitOfWork.Entregables.UpdateDACPAC(request.idEntregable, dacpacPath, cancellationToken);

            // Paso 2: Desplegar el DACPAC en una BD temporal local
            string? tempDatabaseName = null;
            try
            {
                               
                tempDatabaseName = await _dacpacService.DeployDacpacToTemporaryDatabaseAsync(
                    dacpacPath: dacpacPath,
                    cancellationToken: cancellationToken
                );

                // Aquí puedes realizar validaciones o procesos adicionales sobre la BD temporal

                // Por ejemplo: ejecutar scripts de validación, comparar esquemas, etc.

                return $"DACPAC creado en: {dacpacPath}. Base de datos temporal creada: {tempDatabaseName}";
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(tempDatabaseName))
                {
                    try
                    {
                        await _dacpacService.DropTemporaryDatabaseAsync(
                            databaseName: tempDatabaseName,
                            cancellationToken: cancellationToken
                        );
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al eliminar BD temporal {tempDatabaseName}: {ex.Message}");
                    }
                }
            }
        }
    }
}
