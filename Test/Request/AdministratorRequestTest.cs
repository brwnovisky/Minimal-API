using System.Net;
using System.Text;
using System.Text.Json;
using MinimalApi.Domain.Dto;
using MinimalApi.Domain.View;
using Test.Helper;

namespace Test.Request;

[TestClass]
public class AdministratorRequestTest
{
    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        Setup.ClassInit(testContext);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }
    
    [TestMethod]
    public async Task GetAdministratorTest()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "adm@teste.com",
            Password = "123456"
        };

        var content = new StringContent
        (
            JsonSerializer.Serialize(loginDto), 
            Encoding.UTF8,  "Application/json"
        );

        // Act
        var response = await Setup.Client.PostAsync
        (
            "/administrator/login", 
            content
        );

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var admLogado = JsonSerializer.Deserialize<AdminLoggedIn>
        (
            result, 
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        );

        Assert.IsNotNull(admLogado?.Email ?? "");
        Assert.IsNotNull(admLogado?.Profile ?? "");
        Assert.IsNotNull(admLogado?.Token ?? "");

        Console.WriteLine(admLogado?.Token);
    }
}