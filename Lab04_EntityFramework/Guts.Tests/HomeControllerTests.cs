using Guts.Client.XUnit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using School.Web.Controllers;
using School.Web.Infrastructure;
using School.Web.Models;

namespace Guts.Tests;

[ExerciseTestClass("dotNet2", "4-EF", "School", @"School.Web\Controllers\HomeController.cs")]
public class HomeControllerTests
{
    [MonitoredFact]
    public void ShouldHaveConstructorAcceptingIEnrollmentRepositoryAndLogger()
    {
        var constructor = typeof(HomeController).GetConstructor(new[]
        {
            typeof(IEnrollmentRepository),
            typeof(ILogger<HomeController>)
        });

        Assert.True(constructor != null,
            "'HomeController' should have a constructor that accepts 'IEnrollmentRepository' and 'ILogger<HomeController>'.");
    }

    [MonitoredFact]
    public void Index_ShouldCallRepositoryAndPassEnrollmentsToView()
    {
        var repositoryMock = new Mock<IEnrollmentRepository>();
        var loggerMock = new Mock<ILogger<HomeController>>();

        var dummyEnrollments = new List<Enrollment>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Grade = Grade.A,
                Student = new Student { FirstName = "Alice", LastName = "Smith" },
                Course = new Course { Title = "Math" }
            }
        };

        repositoryMock.Setup(r => r.GetAllGradedEnrollmentsWithStudentAndCourse())
            .Returns(dummyEnrollments);

        var controller = new HomeController(repositoryMock.Object, loggerMock.Object);

        var result = controller.Index() as ViewResult;

        Assert.True(result != null, "The 'Index' action should return a 'ViewResult'.");
        repositoryMock.Verify(r => r.GetAllGradedEnrollmentsWithStudentAndCourse(), Times.Once,
            "The 'Index' action must call 'GetAllGradedEnrollmentsWithStudentAndCourse' on the repository.");

        Assert.True(result.Model != null, "The 'Index' action must pass a model to the view.");
        Assert.Same(dummyEnrollments, result.Model);
    }
}
