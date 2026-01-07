using GestionBD.API.Extensions;
using GestionBD.Application;
using GestionBD.Infraestructure;

var builder = WebApplication.CreateBuilder(args);

// Registrar servicios por capas
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddPresentation();

var app = builder.Build();

// Middleware
app.UseExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
