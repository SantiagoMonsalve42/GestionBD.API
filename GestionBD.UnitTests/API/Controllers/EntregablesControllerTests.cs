using GestionBD.API.Controllers;
using GestionBD.Application.Contracts.Entregables;
using GestionBD.Application.Entregables.Commands;
using GestionBD.UnitTests.API.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace GestionBD.UnitTests.API.Controllers;

public sealed class EntregablesControllerTests
{
    [Fact]
    public async Task CreateWithFile_ShouldReturnCreatedAtAction()
    {
        var mediator = new TestMediator();
        mediator.Register<CreateEntregableWithFileCommand, decimal>(_ => 15);

        var controller = new EntregablesController(mediator);

        var request = new CreateEntregableWithFileRequest
        {
            DescripcionEntregable = "Entrega 1",
            IdEjecucion = 2,
            File = CreateFormFile("entregable.zip", "zip-content")
        };

        var result = await controller.CreateWithFile(request, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(EntregablesController.GetById), created.ActionName);
        Assert.Equal(15m, created.RouteValues?["id"]);

        var idValue = GetPropertyValue<decimal>(created.Value, "id");
        Assert.Equal(15m, idValue);
    }

    private static IFormFile CreateFormFile(string fileName, string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);

        return new FormFile(stream, 0, bytes.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/zip"
        };
    }

    private static T GetPropertyValue<T>(object? target, string propertyName)
    {
        var property = target?.GetType().GetProperty(propertyName);
        return property is null ? default! : (T)property.GetValue(target)!;
    }
}