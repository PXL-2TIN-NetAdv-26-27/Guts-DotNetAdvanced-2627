using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace QuizApplication.Tests
{
    // Source https://cezarypiatek.github.io/post/mocking-dependencies-in-asp-net-core/
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly Action<IServiceCollection>? _overrideDependencies;

        public CustomWebApplicationFactory() : this(null)
        {
        }

        internal CustomWebApplicationFactory(Action<IServiceCollection>? overrideDependencies)
        {
            _overrideDependencies = overrideDependencies;
        }

        public static CustomWebApplicationFactory Create(Action<IServiceCollection> overrideDependencies) => new(overrideDependencies);

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                _overrideDependencies?.Invoke(services);
            });
        }
    }
}
