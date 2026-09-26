using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using QuizApplication.Web.Controllers;

namespace QuizApplication.Tests;

[ExerciseTestClass("dotnet2", "5-MVC-EF", "QuizApplication", @"QuizApplication.Web\Controllers\HomeController.cs")]
public class HomeControllerTests
{
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        var mockLogger = new Mock<ILogger<HomeController>>();
        _controller = new HomeController(mockLogger.Object);
    }

    [MonitoredFact]
    public void _01_Index_ShouldReturnDefaultView()
    {
        var result = _controller.Index() as ViewResult;
        Assert.NotNull(result);
        Assert.Null(result.ViewName);
    }

    [MonitoredFact]
    public void _02_About_ReturnsContent()
    {
        var result = _controller.About() as ViewResult;
        Assert.NotNull(result);
    }
}