using GestionBD.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GestionBD.UnitTests.Infrastructure.Data;

public sealed class ApplicationDbContextTests
{
    [Fact]
    public void Create_WithOptions_ExposesDbSets()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;

        using var context = new ApplicationDbContext(options);

        Assert.NotNull(context.TblArtefactos);
        Assert.NotNull(context.TblEjecuciones);
        Assert.NotNull(context.TblEntregables);
        Assert.NotNull(context.TblInstancias);
        Assert.NotNull(context.TblLogEventos);
        Assert.NotNull(context.TblLogTransacciones);
        Assert.NotNull(context.TblMotores);
        Assert.NotNull(context.TblParametros);
    }
}