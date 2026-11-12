using Guts.Client.XUnit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SmurfApp.AppLogic;
using SmurfApp.Domain;
using SmurfApp.Web.Controllers;
using Xunit;

namespace SmurfApp.Web.Tests;

[ExerciseTestClass("dotnet2", "8-ViewsAndForms", "SmurfApp",
    @"SmurfApp.Web\Controllers\HomeController.cs")]
public class HomeControllerTests : IDisposable
{
    private readonly Mock<ILogger<HomeController>> _loggerMock;
    private readonly Mock<ISmurfStore> _smurfStoreMock;
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        _loggerMock = new Mock<ILogger<HomeController>>();
        _smurfStoreMock = new Mock<ISmurfStore>();
        _controller = new HomeController(_loggerMock.Object, _smurfStoreMock.Object);
    }

    public void Dispose()
    {
        _controller?.Dispose();
    }

    [MonitoredFact]
    public void Index_ShouldCallGetAllOnSmurfStore()
    {
        // Arrange
        var smurfs = new List<Smurf>
        {
            new Smurf { Name = "Papa Smurf", Age = 542, Category = Category.Leader },
            new Smurf { Name = "Smurfette", Age = 150, Category = Category.Emotional }
        };
        _smurfStoreMock.Setup(s => s.GetAll()).Returns(smurfs);

        // Act
        var result = _controller.Index() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Same(smurfs, result.Model);
    }

    [MonitoredFact]
    public void Index_ShouldLogInformation()
    {
        // Arrange
        var smurfs = new List<Smurf>();
        _smurfStoreMock.Setup(s => s.GetAll()).Returns(smurfs);

        // Act
        _controller.Index();

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.ToLower().Contains("smurfs")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once,
            "Logger should log an information message when fetching smurfs (the word 'smurfs' should be in the message).");
    }
}
