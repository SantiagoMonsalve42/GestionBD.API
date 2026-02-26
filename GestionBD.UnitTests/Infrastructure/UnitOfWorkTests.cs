using System.Reflection;
using GestionBD.Domain.Entities;
using GestionBD.Infrastructure;
using GestionBD.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Xunit;

namespace GestionBD.UnitTests.Infrastructure;

public sealed class UnitOfWorkTests
{
    [Fact]
    public async Task FindEntityAsync_ExistingEntity_ReturnsEntity()
    {
        var entity = new TblMotore
        {
            IdMotor = 5m,
            NombreMotor = "Motor",
            VersionMotor = "1.0",
            DescripcionMotor = "Desc"
        };

        var dbSetMock = new Mock<DbSet<TblMotore>>();
        dbSetMock.Setup(d => d.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
        var contextMock = new Mock<ApplicationDbContext>(options);
        contextMock.Setup(c => c.Set<TblMotore>()).Returns(dbSetMock.Object);

        var unitOfWork = new UnitOfWork(contextMock.Object);

        var result = await unitOfWork.FindEntityAsync<TblMotore>(entity.IdMotor);

        Assert.Same(entity, result);
    }

    [Fact]
    public async Task CommitTransactionAsync_TransactionExists_CommitsAndDisposes()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
        var contextMock = new Mock<ApplicationDbContext>(options);
        var unitOfWork = new UnitOfWork(contextMock.Object);

        var transactionMock = new Mock<IDbContextTransaction>();
        transactionMock.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        transactionMock.Setup(t => t.DisposeAsync()).Returns(ValueTask.CompletedTask);

        SetTransaction(unitOfWork, transactionMock.Object);

        await unitOfWork.CommitTransactionAsync();

        transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        transactionMock.Verify(t => t.DisposeAsync(), Times.Once);
        Assert.Null(GetTransaction(unitOfWork));
    }

    [Fact]
    public async Task RollbackTransactionAsync_TransactionExists_RollsBackAndDisposes()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
        var contextMock = new Mock<ApplicationDbContext>(options);
        var unitOfWork = new UnitOfWork(contextMock.Object);

        var transactionMock = new Mock<IDbContextTransaction>();
        transactionMock.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        transactionMock.Setup(t => t.DisposeAsync()).Returns(ValueTask.CompletedTask);

        SetTransaction(unitOfWork, transactionMock.Object);

        await unitOfWork.RollbackTransactionAsync();

        transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        transactionMock.Verify(t => t.DisposeAsync(), Times.Once);
        Assert.Null(GetTransaction(unitOfWork));
    }

    private static void SetTransaction(UnitOfWork unitOfWork, IDbContextTransaction transaction)
    {
        var field = typeof(UnitOfWork).GetField("_transaction", BindingFlags.NonPublic | BindingFlags.Instance);
        field!.SetValue(unitOfWork, transaction);
    }

    private static IDbContextTransaction? GetTransaction(UnitOfWork unitOfWork)
    {
        var field = typeof(UnitOfWork).GetField("_transaction", BindingFlags.NonPublic | BindingFlags.Instance);
        return (IDbContextTransaction?)field!.GetValue(unitOfWork);
    }
}