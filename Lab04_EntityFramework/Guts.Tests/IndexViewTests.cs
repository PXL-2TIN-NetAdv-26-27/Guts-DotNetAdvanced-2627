using Guts.Client.XUnit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using School.Web.Infrastructure;
using School.Web.Models;

namespace Guts.Tests;

[ExerciseTestClass("dotNet2", "4-EF", "School", @"School.Web\Views\Home\Index.cshtml")]
public class IndexViewTests
{
    [MonitoredFact]
    public async Task IndexView_ShouldRenderListGroupWithEnrollmentItems()
    {
        var testEnrollments = new List<Enrollment>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Grade = Grade.B,
                Student = new Student { FirstName = "John", LastName = "Doe" },
                Course = new Course { Title = "Chemistry" }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Grade = Grade.A,
                Student = new Student { FirstName = "Jane", LastName = "Smith" },
                Course = new Course { Title = "Calculus" }
            }
        };

        var repositoryMock = new Mock<IEnrollmentRepository>();
        repositoryMock.Setup(r => r.GetAllGradedEnrollmentsWithStudentAndCourse())
            .Returns(testEnrollments);

        await using var factory = CustomWebApplicationFactory.Create(services =>
        {
            services.Replace(ServiceDescriptor.Scoped(_ => repositoryMock.Object));
        });

        var client = factory.CreateClient();

        var response = await client.GetAsync("/");
        response.EnsureSuccessStatusCode();

        var document = await HtmlHelpers.GetDocumentAsync(response);

        var listGroup = document.QuerySelector(".list-group");
        Assert.True(listGroup != null, "The Index view should contain an element with class 'list-group' (e.g. '<ul class=\"list-group\">').");

        var listItems = document.QuerySelectorAll(".list-group .list-group-item").ToList();
        Assert.True(listItems.Count == 2,
            $"Expected 2 '.list-group-item' elements in the list-group, but found {listItems.Count}.");

        // Check first item content
        var firstItemHtml = listItems[0].TextContent;
        Assert.Contains("John", firstItemHtml);
        Assert.Contains("Doe", firstItemHtml);
        Assert.Contains("Chemistry", firstItemHtml);
        Assert.Contains("B", firstItemHtml);

        // Check second item content
        var secondItemHtml = listItems[1].TextContent;
        Assert.Contains("Jane", secondItemHtml);
        Assert.Contains("Smith", secondItemHtml);
        Assert.Contains("Calculus", secondItemHtml);
        Assert.Contains("A", secondItemHtml);
    }
}
