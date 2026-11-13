using System.Net;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Guts.Client.XUnit;
using Microsoft.AspNetCore.Mvc.Testing;
using ToDoListApp.Domain;

namespace Guts.Tests;

[ExerciseTestClass("dotnet2", "10-ToDoListApp", "Web")]
public class WebEndToEndTests : IAsyncLifetime
{
    private CustomWebApplicationFactory _factory = null!;
    private HttpClient? _client;

    public ValueTask InitializeAsync()
    {
        _factory = new CustomWebApplicationFactory();
        try
        {
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        _client?.Dispose();
        await _factory.DisposeAsync();
    }

    private async Task<IHtmlDocument> ParseHtmlAsync(HttpResponseMessage response)
    {
        string html = await response.Content.ReadAsStringAsync();
        var parser = new HtmlParser();
        return await parser.ParseDocumentAsync(html);
    }

    [MonitoredFact]
    public async Task _01_HomePage_ShouldRenderLayoutWithNavLinksPrivacyLinkAndBadge()
    {
        _factory.SeedData(CustomWebApplicationFactory.SeedSampleLists);

        HttpResponseMessage response = await _client.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        IHtmlDocument document = await ParseHtmlAsync(response);
        Assert.NotNull(document.QuerySelector("main"));

        var navLinks = document.QuerySelectorAll("nav a");
        Assert.Contains(navLinks, a => a.TextContent.Trim() == "TODO" && a.GetAttribute("href") == "/");
        Assert.Contains(navLinks, a => a.TextContent.Trim() == "Home" && a.GetAttribute("href") == "/");
        Assert.Contains(navLinks, a => a.TextContent.Trim() == "Search" && a.GetAttribute("href") == "/ToDoList/Search");

        var privacyLink = document.QuerySelector("footer a");
        Assert.NotNull(privacyLink);
        Assert.Equal("/Home/Privacy", privacyLink!.GetAttribute("href"));

        var badge = document.QuerySelector("nav .badge");
        Assert.NotNull(badge);
        Assert.Contains("2", badge!.TextContent);
    }

    [MonitoredFact]
    public async Task _02_SearchPage_Get_ShouldHaveCorrectTitleAndNotShowResults()
    {
        _factory.SeedData(CustomWebApplicationFactory.SeedSampleLists);

        HttpResponseMessage response = await _client.GetAsync("/ToDoList/Search");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        IHtmlDocument document = await ParseHtmlAsync(response);
        Assert.Equal("Search - TODO", document.Title);
        Assert.Contains("No TODO lists found", document.Body!.TextContent);
    }

    [MonitoredFact]
    public async Task _03_SearchPage_Post_WithAndWithoutFilter_ShouldShowMatchingListsAndDetailLinks()
    {
        _factory.SeedData(CustomWebApplicationFactory.SeedSampleLists);

        var emptySearch = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "TitleFilter", "" }
        });
        HttpResponseMessage allResponse = await _client.PostAsync("/ToDoList/Search", emptySearch);
        Assert.Equal(HttpStatusCode.OK, allResponse.StatusCode);

        IHtmlDocument allDoc = await ParseHtmlAsync(allResponse);
        Assert.Contains("Groceries", allDoc.Body!.TextContent);
        Assert.Contains("School Homework", allDoc.Body!.TextContent);

        var detailLinks = allDoc.QuerySelectorAll("a")
            .Where(a => a.TextContent.Trim() == "Show details")
            .ToList();
        Assert.Equal(2, detailLinks.Count);
        Assert.All(detailLinks, link => Assert.StartsWith("/ToDoList/Detail/", link.GetAttribute("href")));

        var filteredSearch = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "TitleFilter", "grocer" }
        });
        HttpResponseMessage filteredResponse = await _client.PostAsync("/ToDoList/Search", filteredSearch);
        Assert.Equal(HttpStatusCode.OK, filteredResponse.StatusCode);

        IHtmlDocument filteredDoc = await ParseHtmlAsync(filteredResponse);
        Assert.Contains("Groceries", filteredDoc.Body!.TextContent);
        Assert.DoesNotContain("School Homework", filteredDoc.Body!.TextContent);
    }

    [MonitoredFact]
    public async Task _04_DetailPage_Get_ExistingList_ShouldShowTitleItemsAndBackLink()
    {
        Guid listId = Guid.Empty;
        _factory.SeedData(db =>
        {
            var list = ToDoList.CreateNew("Weekend Chores");
            list.Items.Add(ToDoItem.CreateNew("Clean kitchen"));
            db.Set<ToDoList>().Add(list);
            listId = list.Id;
        });

        HttpResponseMessage response = await _client.GetAsync($"/ToDoList/Detail/{listId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        IHtmlDocument document = await ParseHtmlAsync(response);
        Assert.Equal("Weekend Chores - TODO", document.Title);
        Assert.Contains("Clean kitchen", document.Body!.TextContent);

        var backLink = document.QuerySelectorAll("a").FirstOrDefault(a => a.TextContent.Trim() == "Back to search");
        Assert.NotNull(backLink);
        Assert.Equal("/ToDoList/Search", backLink!.GetAttribute("href"));
    }

    [MonitoredFact]
    public async Task _05_DetailPage_Get_NonExistingList_ShouldRedirectToHome()
    {
        HttpResponseMessage response = await _client.GetAsync($"/ToDoList/Detail/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/", response.Headers.Location?.ToString());
    }

    [MonitoredFact]
    public async Task _06_DetailPage_Post_ValidItem_ShouldAddItemAndClearInput()
    {
        Guid listId = Guid.Empty;
        _factory.SeedData(db =>
        {
            var list = ToDoList.CreateNew("Weekend Chores");
            db.Set<ToDoList>().Add(list);
            listId = list.Id;
        });

        var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "ListId", listId.ToString() },
            { "NewItemDescription", "Mow the lawn" }
        });

        HttpResponseMessage response = await _client.PostAsync($"/ToDoList/Detail/{listId}", form);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        IHtmlDocument document = await ParseHtmlAsync(response);
        Assert.Contains("Mow the lawn", document.Body!.TextContent);

        var input = document.QuerySelector("input[name='NewItemDescription']") as IHtmlInputElement;
        Assert.NotNull(input);
        Assert.True(string.IsNullOrEmpty(input!.Value));

        var badge = document.QuerySelector("nav .badge");
        Assert.NotNull(badge);
        Assert.Contains("1", badge!.TextContent);
    }

    [MonitoredFact]
    public async Task _07_DetailPage_Post_TooShortDescription_ShouldShowValidationError()
    {
        Guid listId = Guid.Empty;
        _factory.SeedData(db =>
        {
            var list = ToDoList.CreateNew("Weekend Chores");
            db.Set<ToDoList>().Add(list);
            listId = list.Id;
        });

        var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "ListId", listId.ToString() },
            { "NewItemDescription", "abc" }
        });

        HttpResponseMessage response = await _client.PostAsync($"/ToDoList/Detail/{listId}", form);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        IHtmlDocument document = await ParseHtmlAsync(response);
        var validationSpan = document.QuerySelector("span[data-valmsg-for='NewItemDescription']");
        Assert.NotNull(validationSpan);
        Assert.False(string.IsNullOrWhiteSpace(validationSpan!.TextContent));
    }
}
