# Objetivo
Implementar CRUD para todas las entidades del proyecto, siguiendo Clean Architecture y CQRS:
- Commands con Entity Framework Core.
- Queries con Dapper.
- Operaciones: Create, GetAll, GetById, Update, Delete.

# Alcance
- Aplicar a todas las entidades del proyecto de infraestructura (capas `Domain`, `Infrastructure`, `API`).
- Mantener separación de responsabilidades por capas.
- Respetar C# 12 y .NET 8.

# Entidades Identificadas
1. **TblMotore** - Motores de base de datos
2. **TblInstancia** - Instancias de bases de datos
3. **TblEjecucione** - Ejecuciones de scripts
4. **TblEntregable** - Entregables de ejecuciones
5. **TblArtefacto** - Artefactos/scripts SQL
6. **TblLogTransaccione** - Log de transacciones
7. **TblLogEvento** - Log de eventos
8. **TblParametro** - Parámetros del sistema

# Requisitos previos
1. ✅ Confirmar conexión en `appsettings.json` con `DefaultConnection`.
2. ✅ Asegurar `ApplicationDbContext` mapea todas las entidades del dominio.
3. Agregar paquetes NuGet:
   - `Microsoft.EntityFrameworkCore`
   - `Microsoft.EntityFrameworkCore.SqlServer`
   - `Dapper`
   - `System.Data.SqlClient` o `Microsoft.Data.SqlClient`
4. En `Program.cs`:
   - Registrar `ApplicationDbContext` con `AddDbContext`.
   - Registrar `IDbConnection` para Dapper: `SqlConnection` usando `DefaultConnection`.
   - Registrar `MediatR` si se usa (opcional pero recomendado).

# Estructura de proyectos actualizada:
- `GestionBD.Domain`: Entidades del dominio
- `GestionBD.Application`: Commands, Queries, Handlers, DTOs
- `GestionBD.Infrastructure`: DbContext, implementaciones de repositorios
- `GestionBD.API`: Controllers, configuración

# Convenciones de proyecto
- **Commands (EF Core)**: `GestionBD.Application/<Entity>/Commands/...`
- **Queries (Dapper)**: `GestionBD.Application/<Entity>/Queries/...`
- **Handlers**: Uno por comando/query dentro de la misma carpeta.
- **DTOs/Contracts**: `GestionBD.Application/Contracts/<Entity>/...`
- **Controladores API**: `GestionBD.API/Controllers/<Entity>Controller.cs`
- **Infraestructura**:
- DbContext: `GestionBD.Infrastructure/Data/ApplicationDbContext.cs`
- Scripts Dapper: `GestionBD.Infrastructure/Sql/<Entity>/...` (opcional)

# Pasos de Implementación

## 1) ✅ Identificar entidades
- Las 8 entidades están identificadas y mapeadas en `ApplicationDbContext`.
- Todas usan `numeric(18, 0)` para IDs.
- Relaciones:
- TblMotore 1:N TblInstancia
- TblInstancia 1:N TblEjecucione
- TblEjecucione 1:N TblEntregable
- TblEntregable 1:N TblArtefacto
- TblLogTransaccione 1:N TblLogEvento

## 2) Configurar servicios en Program.cs
- En `Program.cs` añadir:
  - Registro de `IDbConnection` para Dapper:
    ```
    builder.Services.AddScoped<IDbConnection>(_ =>
        new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
    ```
  - Registrar `MediatR` si se usa:
    ```
    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
    ```

## 3) Definir contratos/DTOs por entidad
- Crear DTOs para entrada y salida:
  - `Create<Entity>Request`
  - `Update<Entity>Request`
  - `<Entity>Response`

## 4) Commands (EF Core)
Para cada entidad `<Entity>` crear:
- `Create<Entity>Command` + `Create<Entity>CommandHandler`
  - Valida datos, mapea al entity, `DbContext.<Entity>.Add()`, `SaveChangesAsync`.
- `Update<Entity>Command` + `Update<Entity>CommandHandler`
  - Busca por Id, actualiza campos permitidos, `SaveChangesAsync`.
- `Delete<Entity>Command` + `Delete<Entity>CommandHandler`
  - Busca por Id, `Remove`, `SaveChangesAsync`.

Convenciones:
- Input: DTOs.
- Output: Id y/o DTO de respuesta.
- Manejo de errores: retornar `NotFound`/excepciones controladas.

## 5) Queries (Dapper)
Para cada entidad `<Entity>` crear:
- `GetAll<Entity>Query` + `GetAll<Entity>QueryHandler`
  - `SELECT` con columnas explícitas.
- `Get<Entity>ByIdQuery` + `Get<Entity>ByIdQueryHandler`
  - `SELECT ... WHERE Id = @Id`.

Buenas prácticas:
- Usar `QueryAsync<T>` y `QuerySingleOrDefaultAsync<T>`.
- Evitar `SELECT *`.
- Mapear a DTOs de respuesta.

## 6) Controladores API
Para cada entidad `<Entity>`:
- `POST /api/<entity>` -> `Create<Entity>Command`
- `GET /api/<entity>` -> `GetAll<Entity>Query`
- `GET /api/<entity>/{id}` -> `Get<Entity>ByIdQuery`
- `PUT /api/<entity>/{id}` -> `Update<Entity>Command`
- `DELETE /api/<entity>/{id}` -> `Delete<Entity>Command`

Estructura:
- Inyectar `IMediator` (o servicios si no usa MediatR).
- Validar `ModelState`.
- Respuestas:
  - `201 Created` con ubicación al `GET/{id}` al crear.
  - `200 OK` en lecturas.
  - `204 NoContent` en `DELETE` exitoso.
  - `404 NotFound` si no existe.

## 7) SQL para Queries Dapper
- Crear SQL parametrizado por entidad:
  - `Sql/<Entity>/GetAll.sql`
  - `Sql/<Entity>/GetById.sql`
- Ejemplo:

## 8) Transacciones y consistencia
- Commands EF: transacción implícita por `SaveChangesAsync`.
- Si un command requiere múltiples operaciones, usar `IDbContextTransaction`.

## 9) Validaciones
- Usar `FluentValidation` para commands (opcional).
- Validaciones mínimas en DTOs con Data Annotations.

## 10) Pruebas
- Unit tests de handlers (mocks de `DbContext`/`IDbConnection`).
- Integration tests con `WebApplicationFactory`.

# Consideraciones adicionales
- Mantener nombres de tablas y columnas coherentes con el esquema actual.
- Para listas grandes en `GetAll`, considerar paginación, filtros y ordenamiento.
- Activar __EnableOpenAPI__ y Swagger ya configurado.
- Asegurar que el contenedor Docker expone puertos correctos (8080/8081 según `Dockerfile`).
- Revisar políticas de autorización si aplica.

# Checklist de finalización
- [ ] Commands y Queries generados para cada entidad.
- [ ] Controladores por entidad con endpoints CRUD.
- [ ] DTOs consistentes y validados.
- [ ] SQL parametrizado sin `SELECT *`.
- [ ] Mapeos EF Core y migraciones aplicadas.
- [ ] Pruebas unitarias de handlers y endpoints principales.
- [ ] Swagger mostrando todos los endpoints

