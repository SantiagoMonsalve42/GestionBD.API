using GestionBD.Infraestructure.Data;
using GestionBD.API.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;
using GestionBD.Domain;
using GestionBD.Infraestructure;
using GestionBD.Application.Abstractions;
using GestionBD.Infraestructure.Repositories.Query;
using GestionBD.Infraestructure.Services;

var builder = WebApplication.CreateBuilder(args);

// DbContext para Commands (EF Core)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// IDbConnection para Queries (Dapper)
builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

// Unit of Work (para Commands)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// File Storage Service
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

// Read Repositories (para Queries - Dapper)
builder.Services.AddScoped<IArtefactoReadRepository, ArtefactoReadRepository>();
builder.Services.AddScoped<IEntregableReadRepository, EntregableReadRepository>();
builder.Services.AddScoped<IEjecucionReadRepository, EjecucionReadRepository>();
builder.Services.AddScoped<IInstanciaReadRepository, InstanciaReadRepository>();
builder.Services.AddScoped<IMotorReadRepository, MotorReadRepository>();
builder.Services.AddScoped<ILogEventoReadRepository, LogEventoReadRepository>();
builder.Services.AddScoped<ILogTransaccionReadRepository, LogTransaccionReadRepository>();
builder.Services.AddScoped<IParametroReadRepository, ParametroReadRepository>();

// MediatR (CQRS)
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly, 
                                       typeof(GestionBD.Application.AssemblyReference).Assembly));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "GestionBD API",
        Version = "v1",
        Description = "API para gestión de bases de datos con CQRS"
    });
});

var app = builder.Build();
app.UseExceptionHandling();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
