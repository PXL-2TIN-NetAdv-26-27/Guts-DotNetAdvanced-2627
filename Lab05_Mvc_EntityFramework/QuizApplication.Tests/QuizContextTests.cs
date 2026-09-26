using System.Reflection;
using Microsoft.EntityFrameworkCore;
using QuizApplication.Domain;
using QuizApplication.Infrastructure;

namespace QuizApplication.Tests;

[ExerciseTestClass("dotnet2", "5-MVC-EF", "QuizApplication", @"QuizApplication.Infrastructure\QuizDbContext.cs")]
public class QuizContextTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

    public QuizContextTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [MonitoredFact]
    public void _01_ShouldBeDerivedFromDbContext()
    {
        Assert.True(typeof(DbContext).IsAssignableFrom(typeof(QuizDbContext)),
            "'QuizDbContext' should inherit from 'Microsoft.EntityFrameworkCore.DbContext'.");
    }

    [MonitoredFact]
    public void _02_ShouldHave2DbSets()
    {
        var properties = typeof(QuizDbContext).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            .ToList();

        Assert.Equal(2, properties.Count);
        Assert.Contains(properties, p => p.PropertyType == typeof(DbSet<Question>));
        Assert.Contains(properties, p => p.PropertyType == typeof(DbSet<Answer>));
    }

    [MonitoredFact]
    public void _03_OnModelCreating_ShouldSeedQuestionsAndAnswers()
    {
        using var context = _fixture.CreateDbContext();
        var seededQuestions = context.Set<Question>().ToList();
        var seededAnswers = context.Set<Answer>().ToList();

        Assert.True(seededQuestions.Count >= 5,
            $"Expected at least 5 seeded questions, but found {seededQuestions.Count}.");
        Assert.Contains(seededQuestions, q => q.QuestionString == "What is the capital of France?");
        Assert.Contains(seededQuestions, q => q.QuestionString == "What is the largest organ in the human body?");
        Assert.Contains(seededAnswers, a => a.AnswerText == "Rome");
        Assert.Contains(seededAnswers, a => a.AnswerText == "Liver");
    }

    [MonitoredFact]
    public void _04_OnModelCreating_ShouldConfigureTheRelationBetweenQuestionAndAnswers()
    {
        using var context = _fixture.CreateDbContext();
        var answerEntityType = context.Model.FindEntityType(typeof(Answer));
        Assert.NotNull(answerEntityType);

        var answerFk = answerEntityType.GetForeignKeys()
            .FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(Question));
        Assert.NotNull(answerFk);
    }

    [MonitoredFact]
    public void _05_ShouldHaveMigration()
    {
        using var context = _fixture.CreateDbContext();
        var migrations = context.Database.GetMigrations().ToList();

        Assert.True(migrations.Count > 0,
            "Could not find any EF Core migrations. Make sure you created an initial migration (e.g. 'Initial' or 'InitialCreate') in the Infrastructure project.");
    }
}
