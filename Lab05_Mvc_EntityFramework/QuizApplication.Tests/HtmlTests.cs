using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using QuizApplication.AppLogic.Contracts;
using QuizApplication.Domain;

namespace QuizApplication.Tests;

[ExerciseTestClass("dotnet2", "5-MVC-EF", "QuizApplication",
    @"QuizApplication.Web\Views\Home\Index.cshtml;QuizApplication.Web\Views\Quiz\Index.cshtml;QuizApplication.Web\Views\Quiz\QuestionsInCategory.cshtml;QuizApplication.Web\Views\Quiz\QuestionWithAnswers.cshtml")]
public class HtmlTests
{
    [MonitoredTheory]
    [InlineData("/")]
    [InlineData("/Home/About")]
    [InlineData("/Quiz")]
    [InlineData("/Quiz/QuestionsInCategory/1")]
    [InlineData("/Quiz/QuestionWithAnswers/1")]
    public async Task _01_Get_EndpointsReturnSuccessAndCorrectContentTypeAndPage(string url)
    {
        await using var appFactory = new CustomWebApplicationFactory(services =>
        {
            services.Replace(ServiceDescriptor.Scoped(_ =>
            {
                var providerMock = new Mock<IQuizService>();
                providerMock.Setup(x => x.GetQuestionsInCategory(1)).Returns(new List<Question> { new() { Id = 1 } });
                providerMock.Setup(x => x.GetQuestionByIdWithAnswersAndExtra(1)).Returns(new Question { Id = 1 });
                providerMock.Setup(x => x.GetAllCategories()).Returns(new List<Category>());
                return providerMock.Object;
            }));
        });

        var client = appFactory.CreateClient();
        var response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        var content = await HtmlHelpers.GetDocumentAsync(response);
        Assert.NotNull(content.Body);
    }

    [MonitoredFact]
    public async Task _02_HomeControllerIndexShouldShowGif()
    {
        await using var appFactory = new CustomWebApplicationFactory(services =>
        {
            services.Replace(ServiceDescriptor.Scoped(_ =>
            {
                var providerMock = new Mock<IQuizService>();
                return providerMock.Object;
            }));
        });

        var client = appFactory.CreateClient();
        var response = await client.GetAsync("/");
        var content = await HtmlHelpers.GetDocumentAsync(response);

        Assert.NotNull(content.Body);
        Assert.Single(Regex.Matches(content.Body.InnerHtml, "question-asking.gif"));
    }

    [MonitoredFact]
    public async Task _03_QuestionIndexShouldContainHyperLinksToQuestionsInCategory()
    {
        var randomCategoryTexts = new List<string> { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
        var randomCategoryIds = new List<int> { Random.Shared.Next(0, 255), Random.Shared.Next(0, 255) };

        await using var appFactory = new CustomWebApplicationFactory(services =>
        {
            services.Replace(ServiceDescriptor.Scoped(_ =>
            {
                var providerMock = new Mock<IQuizService>();
                providerMock.Setup(x => x.GetAllCategories()).Returns(new List<Category>
                {
                    new() { Id = randomCategoryIds[0], Name = randomCategoryTexts[0] },
                    new() { Id = randomCategoryIds[1], Name = randomCategoryTexts[1] }
                });
                return providerMock.Object;
            }));
        });

        var client = appFactory.CreateClient();
        var response = await client.GetAsync("/Quiz");
        var content = await HtmlHelpers.GetDocumentAsync(response);

        Assert.NotNull(content.Body);
        Assert.Equal(2, Regex.Matches(content.Body.InnerHtml, "href=\"/Quiz/QuestionsInCategory/").Count);
        Assert.Single(Regex.Matches(content.Body.InnerHtml, "href=\"/Quiz/QuestionsInCategory/" + randomCategoryIds[0]));
        Assert.Single(Regex.Matches(content.Body.InnerHtml, "href=\"/Quiz/QuestionsInCategory/" + randomCategoryIds[1]));
        Assert.Single(Regex.Matches(content.Body.InnerHtml, randomCategoryTexts[0]));
        Assert.Single(Regex.Matches(content.Body.InnerHtml, randomCategoryTexts[1]));
    }

    [MonitoredFact]
    public async Task _04_QuestionsInCategoryShouldContainLinksToQuestionsWithAnswers()
    {
        var randomQuestionTexts = new List<string> { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
        var randomQuestionIds = new List<int> { Random.Shared.Next(0, 255), Random.Shared.Next(0, 255) };

        await using var appFactory = new CustomWebApplicationFactory(services =>
        {
            services.Replace(ServiceDescriptor.Scoped(_ =>
            {
                var providerMock = new Mock<IQuizService>();
                providerMock.Setup(x => x.GetQuestionsInCategory(1)).Returns(new List<Question>
                {
                    new() { Id = randomQuestionIds[0], QuestionString = randomQuestionTexts[0] },
                    new() { Id = randomQuestionIds[1], QuestionString = randomQuestionTexts[1] }
                });
                return providerMock.Object;
            }));
        });

        var client = appFactory.CreateClient();
        var response = await client.GetAsync("/Quiz/QuestionsInCategory/1");
        var content = await HtmlHelpers.GetDocumentAsync(response);

        Assert.NotNull(content.Body);
        Assert.Equal(2, Regex.Matches(content.Body.InnerHtml, "href=\"/Quiz/QuestionWithAnswers/").Count);
        Assert.Single(Regex.Matches(content.Body.InnerHtml, "href=\"/Quiz/QuestionWithAnswers/" + randomQuestionIds[0]));
        Assert.Single(Regex.Matches(content.Body.InnerHtml, "href=\"/Quiz/QuestionWithAnswers/" + randomQuestionIds[1]));
        Assert.Single(Regex.Matches(content.Body.InnerHtml, randomQuestionTexts[0]));
        Assert.Single(Regex.Matches(content.Body.InnerHtml, randomQuestionTexts[1]));
        Assert.True(Regex.Matches(content.Body.InnerHtml, "href=\"/Quiz\"").Count >= 1);
    }

    [MonitoredFact]
    public async Task _05_QuestionsWithAnswerShouldShowAllAnswers()
    {
        var randomAnswerTexts = new List<string> { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
        var randomQuestionText = Guid.NewGuid().ToString();
        var randomCategoryId = Random.Shared.Next(0, 255);

        await using var appFactory = new CustomWebApplicationFactory(services =>
        {
            services.Replace(ServiceDescriptor.Scoped(_ =>
            {
                var providerMock = new Mock<IQuizService>();
                providerMock.Setup(x => x.GetQuestionByIdWithAnswersAndExtra(1)).Returns(new Question
                {
                    Id = 1,
                    CategoryId = randomCategoryId,
                    QuestionString = randomQuestionText,
                    Answers = new List<Answer>
                    {
                        new() { AnswerText = randomAnswerTexts[0] },
                        new() { AnswerText = randomAnswerTexts[1] }
                    }
                });
                return providerMock.Object;
            }));
        });

        var client = appFactory.CreateClient();
        var response = await client.GetAsync("/Quiz/QuestionWithAnswers/1");
        var content = await HtmlHelpers.GetDocumentAsync(response);

        Assert.NotNull(content.Body);
        Assert.Single(Regex.Matches(content.Body.InnerHtml, randomQuestionText));
        Assert.Single(Regex.Matches(content.Body.InnerHtml, randomAnswerTexts[0]));
        Assert.Single(Regex.Matches(content.Body.InnerHtml, randomAnswerTexts[1]));
        Assert.True(Regex.Matches(content.Body.InnerHtml, "href=\"/Quiz/QuestionsInCategory/" + randomCategoryId).Count >= 1);
    }
}
