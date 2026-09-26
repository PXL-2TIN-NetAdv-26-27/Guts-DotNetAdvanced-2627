using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using QuizApplication.AppLogic.Contracts;
using QuizApplication.Domain;
using QuizApplication.Web.Controllers;
using QuizApplication.Web.Models;

namespace QuizApplication.Tests;

[ExerciseTestClass("dotnet2", "5-MVC-EF", "QuizApplication", @"QuizApplication.Web\Controllers\QuizController.cs")]
public class QuizControllerTests : IDisposable
{
    private readonly QuizController _controller;
    private readonly Mock<IQuizService> _quizServiceMock = new();

    public QuizControllerTests()
    {
        var mockLogger = new Mock<ILogger<QuizController>>();
        _controller = new QuizController(mockLogger.Object, _quizServiceMock.Object);
    }

    [MonitoredFact]
    public void _01_ShouldHaveAConstructorThatAcceptsQuizService()
    {
        var controllerType = typeof(QuizController);
        ConstructorInfo[] constructors = controllerType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
        Assert.Single(constructors);

        ParameterInfo[] parameters = constructors[0].GetParameters();
        Assert.Equal(2, parameters.Length);
        Assert.Contains(parameters, p => p.ParameterType == typeof(IQuizService));
    }

    [MonitoredFact]
    public void _02_ShouldHaveAPrivateReadonlyIQuizService()
    {
        FieldInfo? listField = typeof(QuizController)
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .SingleOrDefault(field => field.FieldType.IsAssignableTo(typeof(IQuizService)));

        Assert.NotNull(listField);
        Assert.True(listField.IsInitOnly,
            "Make sure the field that holds IQuizService can only be set in the constructor (readonly).");
    }

    [MonitoredFact]
    public void _03_Index_ShouldReturnCategories()
    {
        var categoryList = new List<Category> { new(), new() };
        _quizServiceMock.Setup(service => service.GetAllCategories()).Returns(categoryList);

        var result = _controller.Index() as ViewResult;

        _quizServiceMock.Verify(service => service.GetAllCategories(), Times.Once);
        _quizServiceMock.VerifyNoOtherCalls();

        Assert.NotNull(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Category>>(result.Model);
        Assert.Equal(2, model.Count());
    }

    [MonitoredFact]
    public void _04_QuestionsInCategory_ShouldReturnQuestions()
    {
        var questionList = new List<Question> { new(), new() };
        _quizServiceMock.Setup(service => service.GetQuestionsInCategory(1)).Returns(questionList);

        var result = _controller.QuestionsInCategory(1) as ViewResult;

        _quizServiceMock.Verify(service => service.GetQuestionsInCategory(1), Times.Once);
        _quizServiceMock.VerifyNoOtherCalls();

        Assert.NotNull(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Question>>(result.Model);
        Assert.Equal(2, model.Count());
    }

    [MonitoredFact]
    public void _05_QuestionWithAnswers_ShouldReturnViewWithQuestionViewModel()
    {
        var question = new Question();
        _quizServiceMock.Setup(service => service.GetQuestionByIdWithAnswersAndExtra(1)).Returns(question);

        var result = _controller.QuestionWithAnswers(1) as ViewResult;

        _quizServiceMock.Verify(service => service.GetQuestionByIdWithAnswersAndExtra(1), Times.Once);
        _quizServiceMock.VerifyNoOtherCalls();

        Assert.NotNull(result);
        var model = Assert.IsAssignableFrom<QuestionViewModel>(result.Model);
        Assert.Same(question, model.Question);
    }

    public void Dispose()
    {
        _controller?.Dispose();
    }
}
