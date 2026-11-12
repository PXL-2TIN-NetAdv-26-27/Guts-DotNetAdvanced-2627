using Guts.Client.XUnit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SmurfApp.AppLogic;
using SmurfApp.Domain;
using SmurfApp.Web.Controllers;
using SmurfApp.Web.Models;
using Xunit;

namespace SmurfApp.Web.Tests;

[ExerciseTestClass("dotnet2", "8-ViewsAndForms", "SmurfApp",
    @"SmurfApp.Web\Controllers\SmurfController.cs")]
public class SmurfControllerTests : IDisposable
{
    private readonly Mock<ILogger<SmurfController>> _loggerMock;
    private readonly Mock<ISmurfStore> _smurfStoreMock;
    private readonly SmurfController _controller;

    public SmurfControllerTests()
    {
        _loggerMock = new Mock<ILogger<SmurfController>>();
        _smurfStoreMock = new Mock<ISmurfStore>();
        _controller = new SmurfController(_loggerMock.Object, _smurfStoreMock.Object);
    }

    public void Dispose()
    {
        _controller?.Dispose();
    }

    [MonitoredFact]
    public void AddOrUpdate_HttpGet_WithoutId_ShouldReturnViewWithEmptyModel()
    {
        // Act
        var result = _controller.AddOrUpdate(null) as ViewResult;

        // Assert
        Assert.NotNull(result);
        var model = result!.Model as AddOrUpdateSmurfViewModel;
        Assert.NotNull(model);
        Assert.NotNull(model!.Smurf);
        Assert.Empty(model.Smurf.Name);
    }

    [MonitoredFact]
    public void AddOrUpdate_HttpGet_WithoutId_ShouldLogInformation()
    {
        // Act
        _controller.AddOrUpdate(null);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.ToLower().Contains("new smurf")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once,
            "Logger should log an information message containing the text 'new smurf'");
    }

    [MonitoredFact]
    public void AddOrUpdate_HttpGet_WithValidId_ShouldReturnViewWithExistingSmurf()
    {
        // Arrange
        var smurfId = Guid.NewGuid();
        var existingSmurf = new Smurf
        {
            Id = smurfId,
            Name = "Brainy Smurf",
            Age = 200,
            Category = Category.Skilled
        };
        _smurfStoreMock.Setup(s => s.GetById(smurfId)).Returns(existingSmurf);

        // Act
        var result = _controller.AddOrUpdate(smurfId) as ViewResult;

        // Assert
        Assert.NotNull(result);
        var model = result!.Model as AddOrUpdateSmurfViewModel;
        Assert.NotNull(model);
        _smurfStoreMock.Verify(s => s.GetById(smurfId), Times.Once, "GetById should be called on the SmurfStore");
        Assert.Same(existingSmurf, model!.Smurf);
    }

    [MonitoredFact]
    public void AddOrUpdate_HttpGet_WithValidId_ShouldLogInformation()
    {
        // Arrange
        var smurfId = Guid.NewGuid();
        var existingSmurf = new Smurf { Id = smurfId, Name = "Papa Smurf", Age = 542, Category = Category.Leader };
        _smurfStoreMock.Setup(s => s.GetById(smurfId)).Returns(existingSmurf);

        // Act
        _controller.AddOrUpdate(smurfId);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.ToLower().Contains("existing smurf")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once,
            "Logger should log an information message containing the text 'existing smurf'");
    }

    [MonitoredFact]
    public void AddOrUpdate_HttpGet_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();
        _smurfStoreMock.Setup(s => s.GetById(invalidId)).Returns((Smurf?)null);

        // Act
        var result = _controller.AddOrUpdate(invalidId);

        // Assert
        _smurfStoreMock.Verify(s => s.GetById(invalidId), Times.Once, "GetById should be called on the SmurfStore");
        Assert.IsType<NotFoundResult>(result);
    }

    [MonitoredFact]
    public void AddOrUpdate_HttpGet_WithInvalidId_ShouldLogWarning()
    {
        // Arrange
        var invalidId = Guid.NewGuid();
        _smurfStoreMock.Setup(s => s.GetById(invalidId)).Returns((Smurf?)null);

        // Act
        _controller.AddOrUpdate(invalidId);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.ToLower().Contains("find")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once,
            "Logger should log a warning message containing the text 'find'");
    }

    [MonitoredFact]
    public void AddOrUpdate_HttpPost_WithValidModel_ShouldCallAddOrUpdateOnStore()
    {
        // Arrange
        var smurf = new Smurf
        {
            Id = Guid.NewGuid(),
            Name = "Jokey Smurf",
            Age = 175,
            Category = Category.Playful,
            ImageUrl = "https://example.com/jokey.jpg"
        };
        var model = new AddOrUpdateSmurfViewModel { Smurf = smurf };

        // Act
        _controller.AddOrUpdate(smurf.Id, model);

        // Assert
        _smurfStoreMock.Verify(s => s.AddOrUpdate(smurf), Times.Once, "AddOrUpdate should be called on the SmurfStore");
    }

    [MonitoredFact]
    public void AddOrUpdate_HttpPost_WithValidModel_ShouldRedirectToHomeIndex()
    {
        // Arrange
        var smurf = new Smurf
        {
            Id = Guid.NewGuid(),
            Name = "Vanity Smurf",
            Age = 150,
            Category = Category.Emotional,
            ImageUrl = "https://example.com/vanity.jpg"
        };
        var model = new AddOrUpdateSmurfViewModel { Smurf = smurf };

        // Act
        var result = _controller.AddOrUpdate(smurf.Id, model) as RedirectToActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Index", result!.ActionName);
        Assert.Equal("Home", result.ControllerName);
    }

    [MonitoredFact]
    public void AddOrUpdate_HttpPost_WithValidModel_ShouldLogInformation()
    {
        // Arrange
        var smurf = new Smurf
        {
            Id = Guid.NewGuid(),
            Name = "Handy Smurf",
            Age = 250,
            Category = Category.Skilled,
            ImageUrl = "https://example.com/handy.jpg"
        };
        var model = new AddOrUpdateSmurfViewModel { Smurf = smurf };

        // Act
        _controller.AddOrUpdate(smurf.Id, model);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.ToLower().Contains("adding or updating")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once,
            "Logger should log an information message containing the text 'adding or updating'");
    }

    [MonitoredFact]
    public void AddOrUpdate_HttpPost_WithInvalidModelState_ShouldReturnView()
    {
        // Arrange
        var model = new AddOrUpdateSmurfViewModel
        {
            Smurf = new Smurf { Name = "", Age = 0 }
        };
        _controller.ModelState.AddModelError("Smurf.Name", "Name is required");

        // Act
        var result = _controller.AddOrUpdate(null, model) as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Same(model, result!.Model);
        _smurfStoreMock.Verify(s => s.AddOrUpdate(It.IsAny<Smurf>()), Times.Never,
            "AddOrUpdate should not be called when ModelState is invalid");
    }

    [MonitoredFact]
    public void AddOrUpdate_HttpPost_WithInvalidModelState_ShouldLogWarning()
    {
        // Arrange
        var model = new AddOrUpdateSmurfViewModel
        {
            Smurf = new Smurf { Name = "", Age = 0 }
        };
        _controller.ModelState.AddModelError("Smurf.Name", "Name is required");

        // Act
        _controller.AddOrUpdate(null, model);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once,
            "Logger should log a warning when ModelState is invalid");
    }

    [MonitoredFact]
    public void AddOrUpdate_HttpGetAction_ShouldHaveHttpGetAttribute()
    {
        // Arrange
        var methodInfo = typeof(SmurfController).GetMethods()
            .First(m => m.Name == "AddOrUpdate" && m.GetParameters().Length == 1);

        // Act
        var httpGetAttribute = methodInfo.GetCustomAttributes(typeof(HttpGetAttribute), true)
            .FirstOrDefault() as HttpGetAttribute;

        // Assert
        Assert.NotNull(httpGetAttribute);
    }

    [MonitoredFact]
    public void AddOrUpdate_HttpPostAction_ShouldHaveHttpPostAttribute()
    {
        // Arrange
        var methodInfo = typeof(SmurfController).GetMethods()
            .First(m => m.Name == "AddOrUpdate" && m.GetParameters().Length == 2);

        // Act
        var httpPostAttribute = methodInfo.GetCustomAttributes(typeof(HttpPostAttribute), true)
            .FirstOrDefault() as HttpPostAttribute;

        // Assert
        Assert.NotNull(httpPostAttribute);
    }
}
