using System;
using GestionBD.Utils;
using Microsoft.Data.SqlClient;
using Xunit;

namespace GestionBD.UnitTests.Utils;

public sealed class SqlConnectionStringHelperTests
{
    [Fact]
    public void BuildConnectionString_WhenServerNameEmpty_ShouldThrowArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            SqlConnectionStringHelper.BuildConnectionString(string.Empty, "db"));

        Assert.Equal("serverName", exception.ParamName);
    }

    [Fact]
    public void BuildConnectionString_WhenDatabaseNameEmpty_ShouldThrowArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            SqlConnectionStringHelper.BuildConnectionString("server", " "));

        Assert.Equal("databaseName", exception.ParamName);
    }

    [Fact]
    public void BuildConnectionString_WhenSqlAuthProvided_ShouldSetCredentials()
    {
        var connectionString = SqlConnectionStringHelper.BuildConnectionString(
            "server",
            "db",
            "user",
            "pass");

        var builder = new SqlConnectionStringBuilder(connectionString);

        Assert.Equal("server", builder.DataSource);
        Assert.Equal("db", builder.InitialCatalog);
        Assert.False(builder.IntegratedSecurity);
        Assert.Equal("user", builder.UserID);
        Assert.Equal("pass", builder.Password);
    }

    [Fact]
    public void BuildConnectionString_WhenWindowsAuth_ShouldUseIntegratedSecurity()
    {
        var connectionString = SqlConnectionStringHelper.BuildWindowsAuthConnectionString(
            "server",
            "db");

        var builder = new SqlConnectionStringBuilder(connectionString);

        Assert.Equal("server", builder.DataSource);
        Assert.Equal("db", builder.InitialCatalog);
        Assert.True(builder.IntegratedSecurity);
    }

    [Fact]
    public void BuildConnectionString_WhenOptionsProvided_ShouldApplyOptions()
    {
        var options = new SqlConnectionOptions
        {
            Encrypt = false,
            TrustServerCertificate = false,
            ConnectTimeout = 15,
            CommandTimeout = 120
        };

        var connectionString = SqlConnectionStringHelper.BuildConnectionString(
            "server",
            "db",
            "user",
            "pass",
            options);

        var builder = new SqlConnectionStringBuilder(connectionString);

        Assert.False(builder.Encrypt);
        Assert.False(builder.TrustServerCertificate);
        Assert.Equal(15, builder.ConnectTimeout);
        Assert.Equal(120, builder.CommandTimeout);
    }

    [Fact]
    public void BuildSqlAuthConnectionString_WhenUsernameEmpty_ShouldThrowArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            SqlConnectionStringHelper.BuildSqlAuthConnectionString("server", "db", "", "pass"));

        Assert.Equal("username", exception.ParamName);
    }

    [Fact]
    public void BuildSqlAuthConnectionString_WhenPasswordEmpty_ShouldThrowArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            SqlConnectionStringHelper.BuildSqlAuthConnectionString("server", "db", "user", " "));

        Assert.Equal("password", exception.ParamName);
    }
}