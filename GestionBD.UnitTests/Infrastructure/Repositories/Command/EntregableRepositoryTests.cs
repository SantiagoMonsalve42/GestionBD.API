using GestionBD.Domain.Entities;
using GestionBD.Domain.Enum;
using GestionBD.Infrastructure.Data;
using GestionBD.Infrastructure.Repositories.Command;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GestionBD.UnitTests.Infrastructure.Repositories.Command;

public sealed class EntregableRepositoryTests
{
    [Fact]
    public async Task UpdateEstado_EntregableExists_UpdatesAndReturnsTrue()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        await using var context = await CreateContextAsync(connection);
        var entregable = await SeedEntregableAsync(context);
        var repository = new EntregableRepository(context);

        var result = await repository.UpdateEstado(entregable.IdEntregable, EstadoEntregaEnum.Analisis);

        Assert.True(result);
        Assert.Equal((decimal)EstadoEntregaEnum.Analisis, entregable.IdEstado);
    }

    [Fact]
    public async Task UpdateEstado_EntregableNotFound_ReturnsFalse()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        await using var context = await CreateContextAsync(connection);
        var repository = new EntregableRepository(context);

        var result = await repository.UpdateEstado(999m, EstadoEntregaEnum.Analisis);

        Assert.False(result);
    }

    [Fact]
    public async Task RemoveDatabase_EntregableExists_UpdatesTemporalDbAndReturnsTrue()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        await using var context = await CreateContextAsync(connection);
        var entregable = await SeedEntregableAsync(context);
        var repository = new EntregableRepository(context);

        var result = await repository.removeDatabase(entregable.IdEntregable);

        Assert.True(result);
        context.ChangeTracker.Clear();
        var updated = await context.TblEntregables.SingleAsync(x => x.IdEntregable == entregable.IdEntregable);
        Assert.Equal(string.Empty, updated.TemporalBD);
    }

    [Fact]
    public async Task UpdateDacpac_EntregableExists_UpdatesFieldsAndReturnsTrue()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        await using var context = await CreateContextAsync(connection);
        var entregable = await SeedEntregableAsync(context);
        var repository = new EntregableRepository(context);

        var result = await repository.UpdateDACPAC(entregable.IdEntregable, "nuevo.dacpac", "temporal-db");

        Assert.True(result);
        context.ChangeTracker.Clear();
        var updated = await context.TblEntregables.SingleAsync(x => x.IdEntregable == entregable.IdEntregable);
        Assert.Equal("nuevo.dacpac", updated.RutaDACPAC);
        Assert.Equal("temporal-db", updated.TemporalBD);
    }

    [Fact]
    public async Task UpdateRutaResultado_EntregableExists_UpdatesFieldAndReturnsTrue()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        await using var context = await CreateContextAsync(connection);
        var entregable = await SeedEntregableAsync(context);
        var repository = new EntregableRepository(context);

        var result = await repository.UpdateRutaResultado(entregable.IdEntregable, "resultado.sql");

        Assert.True(result);
        context.ChangeTracker.Clear();
        var updated = await context.TblEntregables.SingleAsync(x => x.IdEntregable == entregable.IdEntregable);
        Assert.Equal("resultado.sql", updated.RutaResultado);
    }

    [Fact]
    public async Task UpdateRutaRollback_EntregableExists_UpdatesFieldAndReturnsTrue()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        await using var context = await CreateContextAsync(connection);
        var entregable = await SeedEntregableAsync(context);
        var repository = new EntregableRepository(context);

        var result = await repository.UpdateRutaRollback(entregable.IdEntregable, "rollback.sql");

        Assert.True(result);
        context.ChangeTracker.Clear();
        var updated = await context.TblEntregables.SingleAsync(x => x.IdEntregable == entregable.IdEntregable);
        Assert.Equal("rollback.sql", updated.RutaRollbackGenerado);
    }

    private static async Task<ApplicationDbContext> CreateContextAsync(SqliteConnection connection)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;
        var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();
        return context;
    }

    private static async Task<TblEntregable> SeedEntregableAsync(ApplicationDbContext context)
    {
        var motor = new TblMotore
        {
            IdMotor = 1m,
            NombreMotor = "Motor",
            VersionMotor = "1.0",
            DescripcionMotor = "Motor de prueba"
        };
        var instancia = new TblInstancia
        {
            IdInstancia = 1m,
            Instancia = "Instancia",
            SessionPath = "usuario",
            IdMotor = motor.IdMotor,
            Puerto = 1433,
            NombreDB = "db",
            IdMotorNavigation = motor
        };
        var ejecucion = new TblEjecucione
        {
            IdEjecucion = 1m,
            IdInstancia = instancia.IdInstancia,
            IdInstanciaNavigation = instancia
        };
        var entregable = new TblEntregable
        {
            IdEntregable = 1m,
            RutaEntregable = "ruta",
            DescripcionEntregable = "descripcion",
            IdEjecucion = ejecucion.IdEjecucion,
            NumeroEntrega = 1,
            RutaDACPAC = "original.dacpac",
            TemporalBD = "temporal",
            IdEstado = 0,
            RutaResultado = "resultado",
            RutaRollbackGenerado = "rollback",
            IdEjecucionNavigation = ejecucion
        };

        context.AddRange(motor, instancia, ejecucion, entregable);
        await context.SaveChangesAsync();

        return entregable;
    }
}
