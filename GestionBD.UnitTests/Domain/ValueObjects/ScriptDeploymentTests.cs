using System.IO.Compression;
using System.Text;
using GestionBD.Domain.Entities;
using GestionBD.Domain.ValueObjects;
using Xunit;

namespace GestionBD.UnitTests.Domain.ValueObjects;

public sealed class ScriptDeploymentTests
{
    [Fact]
    public void Create_WithEmptyScriptName_ShouldThrowArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            ScriptDeployment.Create(string.Empty, "content", 1, "UTF-8"));

        Assert.Equal("scriptName", exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyScriptContent_ShouldThrowArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            ScriptDeployment.Create("script.sql", string.Empty, 1, "UTF-8"));

        Assert.Equal("scriptContent", exception.ParamName);
    }

    [Fact]
    public void Create_WithEncoding_ShouldSetEncoding()
    {
        var result = ScriptDeployment.Create("script.sql", "SELECT 1;", 1, "UTF-16");

        Assert.Equal(Encoding.Unicode, result.Encoding);
    }

    [Fact]
    public void SplitIntoBatches_ShouldSplitOnGoAndRemoveUseStatements()
    {
        var content = """
            USE [MyDb];
            SELECT 1;
            GO
            SELECT 2;
            GO
            """;

        var script = ScriptDeployment.Create("script.sql", content, 1, "UTF-8");

        var batches = script.SplitIntoBatches().ToList();

        Assert.Equal(2, batches.Count);
        Assert.Equal($"SELECT 1;{Environment.NewLine}", batches[0]);
        Assert.Equal($"SELECT 2;{Environment.NewLine}", batches[1]);
        Assert.DoesNotContain("USE", batches[0], StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("USE", batches[1], StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ExtractScriptsFromZip_WithMissingZip_ShouldThrowFileNotFoundException()
    {
        var artefactos = new[]
        {
            new TblArtefacto { NombreArtefacto = "script.sql", Codificacion = "UTF-8", OrdenEjecucion = 1 }
        };

        Assert.Throws<FileNotFoundException>(() =>
            ScriptDeployment.ExtractScriptsFromZip("missing.zip", artefactos));
    }

    [Fact]
    public void ExtractScriptsFromZip_WithNoArtefactos_ShouldThrowInvalidOperationException()
    {
        using var tempZip = new TemporaryZip();

        Assert.Throws<InvalidOperationException>(() =>
            ScriptDeployment.ExtractScriptsFromZip(tempZip.Path, []));
    }

    [Fact]
    public void ExtractScriptsFromZip_ShouldReturnScriptsOrderedByExecution()
    {
        using var tempZip = new TemporaryZip();
        tempZip.AddEntry("script1.sql", "SELECT 1;");
        tempZip.AddEntry("script2.sql", "SELECT 2;");
        
        tempZip.Dispose();

        var artefactos = new[]
        {
            new TblArtefacto { NombreArtefacto = "script2.sql", Codificacion = "UTF-8", OrdenEjecucion = 2, EsReverso = false },
            new TblArtefacto { NombreArtefacto = "script1.sql", Codificacion = "UTF-8", OrdenEjecucion = 1, EsReverso = false },
            new TblArtefacto { NombreArtefacto = "rollback.sql", Codificacion = "UTF-8", OrdenEjecucion = 3, EsReverso = true }
        };

        var scripts = ScriptDeployment.ExtractScriptsFromZip(tempZip.Path, artefactos).ToList();

        Assert.Equal(2, scripts.Count);
        Assert.Equal("script1.sql", scripts[0].ScriptName);
        Assert.Equal("SELECT 1;", scripts[0].ScriptContent);
        Assert.Equal(1, scripts[0].ExecutionOrder);
        Assert.Equal("script2.sql", scripts[1].ScriptName);
        Assert.Equal("SELECT 2;", scripts[1].ScriptContent);
        Assert.Equal(2, scripts[1].ExecutionOrder);
        tempZip.Delete();
    }

    private sealed class TemporaryZip : IDisposable
    {
        private readonly ZipArchive _zipArchive;

        public TemporaryZip()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{Guid.NewGuid()}.zip");
            _zipArchive = ZipFile.Open(Path, ZipArchiveMode.Create);
        }

        public string Path { get; }

        public void AddEntry(string name, string content)
        {
            var entry = _zipArchive.CreateEntry(name);
            using var stream = entry.Open();
            using var writer = new StreamWriter(stream, Encoding.UTF8);
            writer.Write(content);
        }

        public void Dispose()
        {
            _zipArchive.Dispose();
        }
        public void Delete()
        {
            if (File.Exists(Path))
            {
                File.Delete(Path);
            }
        }
    }
}