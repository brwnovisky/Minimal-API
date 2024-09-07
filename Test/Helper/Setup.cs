using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi;
using MinimalApi.Domain.Interface;
using Test.Mock;

namespace Test.Helper;

public class Setup
{
    private const string Port = "5001";
    public static TestContext TestContext = default!;
    private static WebApplicationFactory<Startup> _http = default!;
    public static HttpClient Client = default!;

    public static void ClassInit(TestContext testContext)
    {
        TestContext = testContext;
        var http = new WebApplicationFactory<Startup>();

        _http = http.WithWebHostBuilder
        (
            builder =>
            {
                builder.UseSetting("https_port", Port)
                    .UseEnvironment("Testing");

                builder.ConfigureServices
                (
                    services =>
                    {
                        services.AddScoped
                            <IAdministratorService, AdministratorServiceMock>();
                    }
                );

            }
        );
    
        Client = _http.CreateClient();
    }
    
    public static void ClassCleanup()
    {
        _http.Dispose();
    }
}