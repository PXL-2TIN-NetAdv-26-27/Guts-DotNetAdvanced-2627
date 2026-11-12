using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Guts.Client.XUnit;
using SmurfApp.Web.Tests.Util;
using System.Net;
using Xunit;

namespace SmurfApp.Web.Tests;

[ExerciseTestClass("dotnet2", "8-ViewsAndForms", "SmurfApp",
    @"SmurfApp.Web\Controllers\HomeController.cs")]
public class HomeIntegrationTests : IAsyncLifetime
{
    private CustomWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    public ValueTask InitializeAsync()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    [MonitoredFact]
    public async Task HomePage_ShouldHaveANavigationBarAtTheTop()
    {
        IHtmlDocument document = await GetHomePageHtml();

        var navElement = document.QuerySelector("nav");
        Assert.NotNull(navElement);
        Assert.True(navElement.QuerySelectorAll("a").Length >= 2, "There should be at least 2 links in the navigation bar");
    }

    [MonitoredFact]
    public async Task HomePage_ShouldHaveAFooterAtTheBottom()
    {
        IHtmlDocument document = await GetHomePageHtml();

        var footerElement = document.QuerySelector("footer");
        Assert.NotNull(footerElement);
    }

    [MonitoredFact]
    public async Task HomePage_ShouldShowACardForEachSmurf()
    {
        IHtmlDocument document = await GetHomePageHtml();

        IHtmlCollection<IElement> cards = document.QuerySelectorAll("div.card");
        Assert.True(cards.Length >= 10, "At least 10 smurf cards (div.card) should be found on the home page");

        foreach (var card in cards)
        {
            var nameElement = card.QuerySelector("h5.card-title");
            Assert.NotNull(nameElement);
            Assert.NotEmpty(nameElement!.TextContent);

            var imgElement = card.QuerySelector("img") as IHtmlImageElement;
            Assert.NotNull(imgElement);
            Assert.True(Uri.IsWellFormedUriString(imgElement!.Source, UriKind.Absolute), "Each card image should have a valid absolute URL");

            var detailsLink = card.QuerySelector("a.btn");
            Assert.NotNull(detailsLink);
            string? detailsUrl = detailsLink!.GetAttribute("href");
            Assert.NotNull(detailsUrl);
            Assert.StartsWith("/Smurf/AddOrUpdate/", detailsUrl);
            Assert.True(Guid.TryParse(detailsUrl!.Split('/').Last(), out _), "Each card details link should contain a valid GUID id");
        }
    }

    [MonitoredFact]
    public async Task HomePage_ShouldHaveAButtonToAddANewSmurf()
    {
        IHtmlDocument document = await GetHomePageHtml();

        var addButton = document.QuerySelector("a#add-new");
        Assert.NotNull(addButton);
        Assert.Equal("/Smurf/AddOrUpdate", addButton.GetAttribute("href"));
    }

    [MonitoredFact]
    public async Task HomePage_ShouldHaveASmurfCountInTheNavigation()
    {
        IHtmlDocument document = await GetHomePageHtml();

        var smurfCountSpan = document.QuerySelector("nav span.badge");
        Assert.NotNull(smurfCountSpan);
        Assert.Contains("Smurf Count", smurfCountSpan!.TextContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("10", smurfCountSpan.TextContent, StringComparison.OrdinalIgnoreCase);
    }

    private async Task<IHtmlDocument> GetHomePageHtml()
    {
        HttpResponseMessage response = await _client.GetAsync("/Home/Index");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string content = await response.Content.ReadAsStringAsync();
        var parser = new HtmlParser();
        IHtmlDocument document = parser.ParseDocument(content);
        return document;
    }
}
