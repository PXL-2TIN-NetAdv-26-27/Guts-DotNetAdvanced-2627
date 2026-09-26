using Guts.Client.XUnit;
using Microsoft.Extensions.DependencyInjection;
using QuizApplication.AppLogic;
using QuizApplication.AppLogic.Contracts;
using QuizApplication.Infrastructure;

namespace QuizApplication.Tests;

[ExerciseTestClass("dotnet2", "5-MVC-EF", "QuizApplication", @"QuizApplication.Web\Program.cs")]
public class DependencyInjectionTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public DependencyInjectionTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [MonitoredFact]
    public void ShouldRegisterQuizDbContextInDependencyInjection()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetService<QuizDbContext>();

        Assert.True(dbContext != null,
            "'QuizDbContext' should be registered in the dependency injection container using 'AddDbContext<QuizDbContext>'.");
    }

    [MonitoredFact]
    public void ShouldRegisterQuestionRepositoryInDependencyInjection()
    {
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetService<IQuestionRepository>();

        Assert.True(repository != null,
            "'IQuestionRepository' should be registered in the service container.");
        Assert.IsType<QuestionRepository>(repository);
    }

    [MonitoredFact]
    public void ShouldRegisterCategoryRepositoryInDependencyInjection()
    {
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetService<ICategoryRepository>();

        Assert.True(repository != null,
            "'ICategoryRepository' should be registered in the service container.");
        Assert.IsType<InMemoryCategoryRepository>(repository);
    }

    [MonitoredFact]
    public void ShouldRegisterQuizServiceInDependencyInjection()
    {
        using var scope = _factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetService<IQuizService>();

        Assert.True(service != null,
            "'IQuizService' should be registered in the service container.");
        Assert.IsType<QuizService>(service);
    }
}
