using System.Reflection;
using QuizApplication.AppLogic.Contracts;
using QuizApplication.Domain;
using QuizApplication.Infrastructure;

namespace QuizApplication.Tests;

[ExerciseTestClass("dotnet2", "5-MVC-EF", "QuizApplication", @"QuizApplication.Infrastructure\InMemoryCategoryRepository.cs")]
public class InMemoryCategoryRepositoryTests
{
    [MonitoredFact]
    public void _01_ShouldImplementICategoryRepository()
    {
        Assert.True(typeof(ICategoryRepository).IsAssignableFrom(typeof(InMemoryCategoryRepository)),
            "'InMemoryCategoryRepository' should implement 'ICategoryRepository'.");
    }

    [MonitoredFact]
    public void _02_ShouldHaveAPrivateReadonlyIListField()
    {
        FieldInfo? listField = typeof(InMemoryCategoryRepository)
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .SingleOrDefault(field => field.FieldType.IsAssignableTo(typeof(IReadOnlyList<Category>)));

        Assert.NotNull(listField);
        Assert.True(listField.IsInitOnly,
            "Make sure the field that holds the collection of categories can only be set in the constructor (readonly).");
    }

    [MonitoredFact]
    public void _03_ShouldNotHavePublicProperties()
    {
        var props = typeof(InMemoryCategoryRepository).GetProperties();
        Assert.Empty(props);
    }
}
