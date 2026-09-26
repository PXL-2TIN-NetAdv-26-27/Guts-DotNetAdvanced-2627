using System.Reflection;
using Moq;
using QuizApplication.AppLogic;
using QuizApplication.AppLogic.Contracts;
using QuizApplication.Domain;

namespace QuizApplication.Tests;

[ExerciseTestClass("dotnet2", "5-MVC-EF", "QuizApplication", @"QuizApplication.AppLogic\QuizService.cs")]
public class QuizServiceTests
{
    private readonly Type _quizServiceType = typeof(QuizService);
    private readonly Mock<IQuestionRepository> _questionRepositoryMock = new();
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();

    [MonitoredFact]
    public void _01_ShouldImplementIQuizService()
    {
        Assert.True(typeof(IQuizService).IsAssignableFrom(_quizServiceType),
            "The 'IQuizService' should be implemented by 'QuizService'.");
    }

    [MonitoredFact]
    public void _02_ShouldOnlyBeVisibleToTheAppLogicLayer()
    {
        Assert.True(_quizServiceType.IsNotPublic,
            "Only IQuizService should be visible to other layers. The QuizService class itself should be internal to AppLogic.");
    }

    [MonitoredFact]
    public void _03_ShouldHaveAConstructorThatAcceptsTwoRepositories()
    {
        ConstructorInfo[] constructors = _quizServiceType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
        Assert.Single(constructors);

        ParameterInfo[] parameters = constructors[0].GetParameters();
        Assert.Equal(2, parameters.Length);
        Assert.Contains(parameters, p => p.ParameterType == typeof(ICategoryRepository));
        Assert.Contains(parameters, p => p.ParameterType == typeof(IQuestionRepository));
    }

    [MonitoredFact]
    public void _04_GetAllCategoriesShouldInvokeRepositoryAndReturnCategories()
    {
        QuizService service = ConstructQuizService(_questionRepositoryMock.Object, _categoryRepositoryMock.Object);

        var categoryList = new List<Category> { new(), new() };
        _categoryRepositoryMock.Setup(repo => repo.GetAll()).Returns(categoryList);

        var returnedList = service.GetAllCategories();

        _categoryRepositoryMock.Verify(repo => repo.GetAll(), Times.Once);
        _categoryRepositoryMock.VerifyNoOtherCalls();
        _questionRepositoryMock.VerifyNoOtherCalls();

        Assert.NotNull(returnedList);
        Assert.Equal(2, returnedList.Count);
    }

    [MonitoredFact]
    public void _05_GetQuestionsInCategoryShouldInvokeRepositoryAndReturnQuestions()
    {
        QuizService service = ConstructQuizService(_questionRepositoryMock.Object, _categoryRepositoryMock.Object);

        var randomCategoryId = Random.Shared.Next(1, 1000);
        var mockQuestion1 = new Question { CategoryId = randomCategoryId };
        var mockQuestion2 = new Question { CategoryId = randomCategoryId };
        var questionList = new List<Question> { mockQuestion1, mockQuestion2 };

        _questionRepositoryMock.Setup(repo => repo.GetByCategoryId(randomCategoryId)).Returns(questionList);

        var returnedQuestions = service.GetQuestionsInCategory(randomCategoryId);

        _questionRepositoryMock.Verify(repo => repo.GetByCategoryId(randomCategoryId), Times.Once);
        _questionRepositoryMock.VerifyNoOtherCalls();
        _categoryRepositoryMock.VerifyNoOtherCalls();

        Assert.NotNull(returnedQuestions);
        Assert.Equal(2, returnedQuestions.Count);
        Assert.Equal(randomCategoryId, returnedQuestions[0].CategoryId);
    }

    [MonitoredFact]
    public void _06_GetQuestionByIdShouldInvokeRepositoryAndReturnQuestion()
    {
        QuizService service = ConstructQuizService(_questionRepositoryMock.Object, _categoryRepositoryMock.Object);

        var mockQuestion = new Question { Answers = new List<Answer> { new(), new() } };
        _questionRepositoryMock.Setup(repo => repo.GetByIdWithAnswers(1)).Returns(mockQuestion);

        var returnedQuestion = service.GetQuestionByIdWithAnswersAndExtra(1);

        _questionRepositoryMock.Verify(repo => repo.GetByIdWithAnswers(1), Times.Once);
        _questionRepositoryMock.VerifyNoOtherCalls();
        _categoryRepositoryMock.VerifyNoOtherCalls();

        Assert.NotNull(returnedQuestion);
        Assert.Equal(3, returnedQuestion.Answers.Count);
    }

    [MonitoredTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void _07_GetQuestionByIdShouldAddAnExtraAnswer(bool correctAnswerPresent)
    {
        QuizService service = ConstructQuizService(_questionRepositoryMock.Object, _categoryRepositoryMock.Object);

        var mockQuestion = new Question
        {
            Answers = new List<Answer>
            {
                new() { IsCorrect = correctAnswerPresent },
                new()
            }
        };
        _questionRepositoryMock.Setup(repo => repo.GetByIdWithAnswers(1)).Returns(mockQuestion);

        var returnedQuestion = service.GetQuestionByIdWithAnswersAndExtra(1);

        _questionRepositoryMock.Verify(repo => repo.GetByIdWithAnswers(1), Times.Once);
        _questionRepositoryMock.VerifyNoOtherCalls();
        _categoryRepositoryMock.VerifyNoOtherCalls();

        Assert.NotNull(returnedQuestion);
        Assert.Equal(3, returnedQuestion.Answers.Count);
        Assert.Contains("the answers", returnedQuestion.Answers.Last().AnswerText, StringComparison.OrdinalIgnoreCase);

        if (correctAnswerPresent)
        {
            Assert.False(returnedQuestion.Answers.Last().IsCorrect,
                "If one or more correct answers are present, the extra answer should have IsCorrect == false.");
        }
        else
        {
            Assert.True(returnedQuestion.Answers.Last().IsCorrect,
                "If no correct answers are present, the extra answer should have IsCorrect == true.");
        }
    }

    private static QuizService ConstructQuizService(IQuestionRepository questionRepository, ICategoryRepository categoryRepository)
    {
        QuizService? service = null;
        try
        {
            service = Activator.CreateInstance(typeof(QuizService),
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
            [questionRepository, categoryRepository], null) as QuizService
            ?? Activator.CreateInstance(typeof(QuizService),
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
            [categoryRepository, questionRepository], null) as QuizService
            ?? Activator.CreateInstance(typeof(QuizService),
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
            Array.Empty<object>(), null) as QuizService;
        }
        catch (Exception ex)
        {
            Assert.Fail($"Failed to instantiate QuizService: {ex.Message}");
        }

        Assert.True(service is not null, "Failed to instantiate QuizService...");

        return service;
    }
}
