using GestionBD.Domain.Entities;
using GestionBD.Infrastructure.Data;
using GestionBD.Infrastructure.Repositories.Command;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GestionBD.UnitTests.Infrastructure.Repositories.Command;

public sealed class ArtefactoRepositoryTests
{
    [Fact]
    public async Task UpdateOrder_AllItemsExist_UpdatesOrderAndReturnsTrue()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        await using var context = await CreateContextAsync(connection);
        var artefactos = await SeedArtefactosAsync(context);
        var repository = new ArtefactoRepository(context);
        var listado = artefactos.ToDictionary(a => a.IdArtefacto, a => a.OrdenEjecucion);

        var result = await repository.UpdateOrder(listado);

        Assert.True(result);
        context.ChangeTracker.Clear();
        foreach (var item in listado)
        {
            var updated = await context.TblArtefactos.SingleAsync(x => x.IdArtefacto == item.Key);
            Assert.Equal(item.Value, updated.OrdenEjecucion);
        }
    }

    [Fact]
    public async Task UpdateOrder_ArtefactoNotFound_ThrowsKeyNotFoundException()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        await using var context = await CreateContextAsync(connection);
        var repository = new ArtefactoRepository(context);
        var listado = new Dictionary<decimal, int>
        {
            [999m] = 1
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => repository.UpdateOrder(listado));
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

    private static async Task<IReadOnlyList<TblArtefacto>> SeedArtefactosAsync(ApplicationDbContext context)
    {
        var entregable = await SeedEntregableAsync(context);
        var artefacto1 = new TblArtefacto
        {
            IdArtefacto = 1m,
            IdEntregable = entregable.IdEntregable,
            OrdenEjecucion = 1,
            Codificacion = "UTF8",
            NombreArtefacto = "script1.sql",
            RutaRelativa = "scripts",
            EsReverso = false,
            IdEntregableNavigation = entregable
        };
        var artefacto2 = new TblArtefacto
        {
            IdArtefacto = 2m,
            IdEntregable = entregable.IdEntregable,
            OrdenEjecucion = 2,
            Codificacion = "UTF8",
            NombreArtefacto = "script2.sql",
            RutaRelativa = "scripts",
            EsReverso = false,
            IdEntregableNavigation = entregable
        };

        context.TblArtefactos.AddRange(artefacto1, artefacto2);
        await context.SaveChangesAsync();

        return new List<TblArtefacto> { artefacto1, artefacto2 };
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
            IdEjecucionNavigation = ejecucion
        };

        context.AddRange(motor, instancia, ejecucion, entregable);
        await context.SaveChangesAsync();

        return entregable;
    }
}
