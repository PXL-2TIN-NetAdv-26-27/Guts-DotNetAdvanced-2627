using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using School.Web;

namespace Guts.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly Action<IServiceCollection>? _overrideServices;

    public CustomWebApplicationFactory()
    {
    }

    internal CustomWebApplicationFactory(Action<IServiceCollection>? overrideServices)
    {
        _overrideServices = overrideServices;
    }

    public static CustomWebApplicationFactory Create(Action<IServiceCollection> overrideServices) => new(overrideServices);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            _overrideServices?.Invoke(services);
        });
    }
}
