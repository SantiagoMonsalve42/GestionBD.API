using GestionBD.API.Configuration;
using GestionBD.API.Extensions;
using GestionBD.Application;
using GestionBD.Infraestructure;
using GestionBD.Infraestructure.ExternalServices;

var builder = WebApplication.CreateBuilder(args);

// Servicios externos (Vault, Keycloak settings)
await builder.Services.AddExternalServicesAsync(builder.Configuration);

// Infraestructura
builder.Services.AddInfrastructure(builder.Configuration);

// Application
builder.Services.AddApplication();

// Presentation (Controllers, Swagger, CORS)
builder.Services.AddPresentation(builder.Configuration);

// Autenticación con Keycloak
builder.Services.AddKeycloakAuthentication();

var app = builder.Build();

// Middleware
app.UseExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS
var corsSettings = builder.Configuration.GetSection(CorsSettings.SectionName).Get<CorsSettings>();
if (corsSettings is not null)
{
    app.UseCors(corsSettings.PolicyName);
}

// Autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
