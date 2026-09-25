using AngleSharp;
using AngleSharp.Html.Dom;

namespace Guts.Tests;

public static class HtmlHelpers
{
    public static async Task<IHtmlDocument> GetDocumentAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var context = BrowsingContext.New(Configuration.Default);
        return await context.OpenAsync(req => req.Content(content)) as IHtmlDocument
               ?? throw new InvalidOperationException("Could not parse HTML document.");
    }
}

