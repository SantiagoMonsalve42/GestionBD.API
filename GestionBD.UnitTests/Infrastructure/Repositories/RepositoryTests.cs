using GestionBD.Domain.Entities;
using GestionBD.Infrastructure.Data;
using GestionBD.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace GestionBD.UnitTests.Infrastructure.Repositories;

public sealed class RepositoryTests
{
    [Fact]
    public void Add_ValidEntity_AddsToDbSet()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
        var contextMock = new Mock<ApplicationDbContext>(options);
        var dbSetMock = new Mock<DbSet<TblMotore>>();
        contextMock.Setup(c => c.Set<TblMotore>()).Returns(dbSetMock.Object);

        var repository = new Repository<TblMotore>(contextMock.Object);
        var entity = new TblMotore
        {
            IdMotor = 1m,
            NombreMotor = "Motor",
            VersionMotor = "1.0",
            DescripcionMotor = "Desc"
        };

        repository.Add(entity);

        dbSetMock.Verify(d => d.Add(entity), Times.Once);
    }

    [Fact]
    public void Update_ValidEntity_UpdatesDbSet()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
        var contextMock = new Mock<ApplicationDbContext>(options);
        var dbSetMock = new Mock<DbSet<TblMotore>>();
        contextMock.Setup(c => c.Set<TblMotore>()).Returns(dbSetMock.Object);

        var repository = new Repository<TblMotore>(contextMock.Object);
        var entity = new TblMotore
        {
            IdMotor = 1m,
            NombreMotor = "Motor",
            VersionMotor = "1.0",
            DescripcionMotor = "Desc"
        };

        repository.Update(entity);

        dbSetMock.Verify(d => d.Update(entity), Times.Once);
    }

    [Fact]
    public void Remove_ValidEntity_RemovesFromDbSet()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
        var contextMock = new Mock<ApplicationDbContext>(options);
        var dbSetMock = new Mock<DbSet<TblMotore>>();
        contextMock.Setup(c => c.Set<TblMotore>()).Returns(dbSetMock.Object);

        var repository = new Repository<TblMotore>(contextMock.Object);
        var entity = new TblMotore
        {
            IdMotor = 1m,
            NombreMotor = "Motor",
            VersionMotor = "1.0",
            DescripcionMotor = "Desc"
        };

        repository.Remove(entity);

        dbSetMock.Verify(d => d.Remove(entity), Times.Once);
    }
}