using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Domain.Entity;
using MinimalApi.Domain.Service;
using MinimalApi.Infrastructure.Db;

namespace Test.Domain.Service;

[TestClass]
public class AdministratorServiceTest
{
    private Context CreateContextTest()
    {
        var assemblyPath = Path.GetDirectoryName
        (
            Assembly.GetExecutingAssembly().Location
        );
        
        var path = Path.GetFullPath
        (
            Path.Combine(assemblyPath ?? "", "..", "..", "..")
        );

        var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile
            (
                "appsettings.json", 
                optional: false, 
                reloadOnChange: true
            )
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        return new Context(configuration);
    }


    [TestMethod]
    public void AddAdministratorTest()
    {
        // Arrange
        var context = CreateContextTest();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators");

        var adm = new Administrator
        {
            Email = "teste@teste.com",
            Password = "teste",
            Profile = "Adm"
        };

        var administratorService = new AdministratorService(context);

        // Act
        administratorService.Add(adm);

        // Assert
        Assert.AreEqual(1, administratorService.GetAll(1).Count());
    }

    [TestMethod]
    public void GetByIdTest()
    {
        // Arrange
        var context = CreateContextTest();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators");

        var adm = new Administrator
        {
            Email = "teste@teste.com",
            Password = "teste",
            Profile = "Adm"
        };

        var administratorService = new AdministratorService(context);

        // Act
        administratorService.Add(adm);
        var admGot = administratorService.GetById(adm.Id);

        // Assert
        Assert.AreEqual(1, admGot?.Id);
    }
}