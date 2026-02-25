using GestionBD.Domain.ValueObjects;
using Xunit;

namespace GestionBD.UnitTests.Domain.ValueObjects;

public sealed class SqlValidationTests
{
    [Fact]
    public void Valid_ShouldReturnValidInstanceWithEmptyCollections()
    {
        var result = SqlValidation.Valid();

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.Empty(result.Warnings);
        Assert.Empty(result.Suggestions);
        Assert.False(result.HasErrors);
        Assert.False(result.HasWarnings);
        Assert.False(result.HasSuggestions);
    }

    [Fact]
    public void Invalid_WithErrors_ShouldSetIsValidFalseAndExposeErrors()
    {
        var error = new ValidationError("E001", "Error de validación");

        var result = SqlValidation.Invalid(error);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(error, result.Errors[0]);
        Assert.True(result.HasErrors);
        Assert.False(result.HasWarnings);
        Assert.False(result.HasSuggestions);
    }

    [Fact]
    public void Create_WithNullCollections_ShouldDefaultToEmptyLists()
    {
        var result = SqlValidation.Create(true, null, null, null);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.Empty(result.Warnings);
        Assert.Empty(result.Suggestions);
    }

    [Fact]
    public void Create_WithWarningsAndSuggestions_ShouldExposeFlags()
    {
        var warnings = new[] { new ValidationWarning("W001", "Advertencia") };
        var suggestions = new[] { new ValidationSuggestion("S001", "Sugerencia") };

        var result = SqlValidation.Create(true, [], warnings, suggestions);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.Single(result.Warnings);
        Assert.Single(result.Suggestions);
        Assert.False(result.HasErrors);
        Assert.True(result.HasWarnings);
        Assert.True(result.HasSuggestions);
    }
}