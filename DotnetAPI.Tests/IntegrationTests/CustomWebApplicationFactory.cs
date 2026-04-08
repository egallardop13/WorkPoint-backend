using DotnetAPI.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DotnetAPI.Tests.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("AppSettings:TokenKey",
            "ThisIsAFakeTokenKeyForTestingPurposesOnlyItMustBeAtLeast64BytesLongToWork!!");
        builder.UseSetting("AppSettings:PasswordKey", "FakePasswordKeyForTesting");

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IDataContextDapper));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddScoped<IDataContextDapper>(_ => Mock.Of<IDataContextDapper>());
        });

        builder.UseEnvironment("Testing");
    }
}
