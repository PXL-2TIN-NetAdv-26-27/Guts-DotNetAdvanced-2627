using QuizApplication.AppLogic.Contracts;
using QuizApplication.Domain;
using QuizApplication.Infrastructure;

namespace QuizApplication.Tests;

[ExerciseTestClass("dotnet2", "5-MVC-EF", "QuizApplication", @"QuizApplication.Infrastructure\QuestionRepository.cs")]
public class QuestionRepositoryTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

    public QuestionRepositoryTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [MonitoredFact]
    public void _01_ShouldImplementIQuestionRepository()
    {
        Assert.True(typeof(IQuestionRepository).IsAssignableFrom(typeof(QuestionRepository)),
            "'QuestionRepository' should implement 'IQuestionRepository'.");
    }

    [MonitoredFact]
    public void _02_GetByCategoryId_ShouldReturnQuestionsInCategory()
    {
        using var context = _fixture.CreateDbContext();
        IQuestionRepository repository = new QuestionRepository(context);

        string guid = Guid.NewGuid().ToString();
        Question q1 = new Question() { Id = 1024, QuestionString = guid, CategoryId = 99 };
        string guid2 = Guid.NewGuid().ToString();
        Question q2 = new Question() { Id = 1025, QuestionString = guid2, CategoryId = 99 };
        context.Questions.Add(q1);
        context.Questions.Add(q2);
        context.SaveChanges();

        IReadOnlyList<Question> questionsInCategory = repository.GetByCategoryId(99);

        Assert.Equal(2, questionsInCategory.Count);
        Assert.Contains(questionsInCategory, q => q.QuestionString == guid);
        Assert.Contains(questionsInCategory, q => q.QuestionString == guid2);
    }

    [MonitoredFact]
    public void _03_GetById_ShouldReturnQuestionWithAnswers()
    {
        using var context = _fixture.CreateDbContext();
        IQuestionRepository repository = new QuestionRepository(context);

        Question? question = repository.GetByIdWithAnswers(7);

        Assert.NotNull(question);
        Assert.Equal("What is the largest mammal in the world?", question.QuestionString);
        Assert.NotNull(question.Answers);
        Assert.Equal(4, question.Answers.Count);
    }
}
