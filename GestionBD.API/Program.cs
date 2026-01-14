using GestionBD.API.Configuration;
using GestionBD.API.Extensions;
using GestionBD.Application;
using GestionBD.Infraestructure;
using GestionBD.Infraestructure.ExternalServices;

var builder = WebApplication.CreateBuilder(args);
await builder.Services.AddExternalServicesAsync(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddPresentation(builder.Configuration);

var app = builder.Build();

// Middleware
app.UseExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Obtener el nombre de la política CORS desde configuración
var corsSettings = builder.Configuration.GetSection(CorsSettings.SectionName).Get<CorsSettings>();
if (corsSettings is not null)
{
    app.UseCors(corsSettings.PolicyName);
}

app.UseAuthorization();
app.MapControllers();

app.Run();
