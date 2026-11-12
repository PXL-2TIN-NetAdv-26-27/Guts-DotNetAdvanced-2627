using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Guts.Client.XUnit;
using SmurfApp.Web.Tests.Util;
using System.Net;
using Xunit;

namespace SmurfApp.Web.Tests;

[ExerciseTestClass("dotnet2", "8-ViewsAndForms", "SmurfApp",
    @"SmurfApp.Web\Controllers\SmurfController.cs")]
public class SmurfIntegrationTests : IAsyncLifetime
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
    public async Task AddOrUpdatePage_NoSmurfId_ShouldHaveAForm()
    {
        IHtmlDocument document = await GetAddOrUpdatePageHtml();

        IHtmlFormElement? form = document.QuerySelector("form") as IHtmlFormElement;
        Assert.NotNull(form);
        ValidateFormElements(form!);
    }

    [MonitoredFact]
    public async Task AddOrUpdatePage_ExistingSmurfId_ShouldHaveAForm()
    {
        var papaSmurfId = Guid.Parse("7bd2c98e-8e90-4f50-9f7e-2fd9b5ce36f0");
        IHtmlDocument document = await GetAddOrUpdatePageHtml(papaSmurfId);

        IHtmlFormElement? form = document.QuerySelector("form") as IHtmlFormElement;
        Assert.NotNull(form);
        ValidateFormElements(form!);
    }

    private static void ValidateFormElements(IHtmlFormElement form)
    {
        var labels = form.QuerySelectorAll("label");
        Assert.Equal(4, labels.Length);
        foreach (var label in labels)
        {
            Assert.False(string.IsNullOrEmpty(label.GetAttribute("for")), "All labels should have a 'for' attribute");
        }

        var nameInput = form.QuerySelector("input[name='Smurf.Name']") as IHtmlInputElement;
        Assert.NotNull(nameInput);

        var ageInput = form.QuerySelector("input[name='Smurf.Age']") as IHtmlInputElement;
        Assert.NotNull(ageInput);

        var imageUrlInput = form.QuerySelector("input[name='Smurf.ImageUrl']") as IHtmlInputElement;
        Assert.NotNull(imageUrlInput);

        var categorySelect = form.QuerySelector("select[name='Smurf.Category']") as IHtmlSelectElement;
        Assert.NotNull(categorySelect);
        var options = categorySelect.QuerySelectorAll("option");
        string[] expectedCategories = Enum.GetNames<Domain.Category>();
        Assert.True(expectedCategories.All(ec => options.Any(o => o.GetAttribute("value") == ec)), "The select should have options for all categories");
        Assert.Equal("-- Select a category --", options[0].TextContent.Trim());
        Assert.True(options[0].HasAttribute("disabled"), "The first option in the select should be disabled");
    }

    private async Task<IHtmlDocument> GetAddOrUpdatePageHtml(Guid? smurfId = null)
    {
        string url = smurfId is null ? "/Smurf/AddOrUpdate" : $"/Smurf/AddOrUpdate/{smurfId}";

        HttpResponseMessage response = await _client.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string content = await response.Content.ReadAsStringAsync();
        var parser = new HtmlParser();
        IHtmlDocument document = parser.ParseDocument(content);
        return document;
    }
}
