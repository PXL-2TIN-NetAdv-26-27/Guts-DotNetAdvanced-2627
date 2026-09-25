using Guts.Client.XUnit;
using Microsoft.Extensions.DependencyInjection;
using School.Web.Infrastructure;

namespace Guts.Tests;

[ExerciseTestClass("dotNet2", "4-EF", "School", @"School.Web\Program.cs")]
public class DependencyInjectionTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public DependencyInjectionTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [MonitoredFact]
    public void ShouldRegisterSchoolDbContextInDependencyInjection()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetService<SchoolDbContext>();

        Assert.True(dbContext != null,
            "'SchoolDbContext' should be registered in the dependency injection container using 'AddDbContext<SchoolDbContext>'.");
    }

    [MonitoredFact]
    public void ShouldRegisterEnrollmentDbRepositoryAsScoped()
    {
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetService<IEnrollmentRepository>();

        Assert.True(repository != null,
            "'IEnrollmentRepository' should be registered in the service container.");
        Assert.IsType<EnrollmentDbRepository>(repository);
    }
}
