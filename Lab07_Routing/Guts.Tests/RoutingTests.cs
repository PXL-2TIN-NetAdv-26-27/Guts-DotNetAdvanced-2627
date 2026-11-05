using System.Net;
using System.Text.RegularExpressions;
using AngleSharp.Html.Dom;
using Guts.Client.Core.TestTools;
using Guts.Client.XUnit;
using Microsoft.AspNetCore.Mvc.Testing;
using Route98.Web;
using Xunit;

namespace Guts.Tests;

[ExerciseTestClass("dotNet2", "7-Routing", "Route98",
    @"Route98.Web\Program.cs;Route98.Web\Views\Shared\_Layout.cshtml;Route98.Web\Views\Home\Index.cshtml")]
public class RoutingTests
{
    [MonitoredFact]
    public async Task _01_Get_Root_ShouldMapToHomeIndex()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var document = await HtmlHelpers.GetDocumentAsync(response);
        var heading = document.QuerySelector("h1.display-4");
        Assert.NotNull(heading);
        Assert.Equal("Welcome", heading.TextContent.Trim());
    }

    [MonitoredFact]
    public async Task _02_Get_PrivacyOfHome_ShouldMapToHomePrivacy()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/privacy/of/home");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var document = await HtmlHelpers.GetDocumentAsync(response);
        var heading = document.QuerySelector("h1");
        Assert.NotNull(heading);
        Assert.Equal("Privacy Policy", heading.TextContent.Trim());
    }

    [MonitoredFact]
    public async Task _03_Get_DetailOfProduct_WithValidGuid_ShouldMapToProductDetail()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        var productId = Guid.NewGuid();

        var response = await client.GetAsync($"/detail/of/product/{productId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var document = await HtmlHelpers.GetDocumentAsync(response);
        var heading = document.QuerySelector("h1.display-4");
        Assert.NotNull(heading);
        Assert.Contains(productId.ToString(), heading.TextContent);
    }

    [MonitoredFact]
    public async Task _04_Get_DetailOfProduct_WithInvalidGuid_ShouldReturnNotFound()
    {
        try
        {
            await _03_Get_DetailOfProduct_WithValidGuid_ShouldMapToProductDetail();
        }
        catch (Exception)
        {
            Assert.Fail($"Make sure test '{nameof(_03_Get_DetailOfProduct_WithValidGuid_ShouldMapToProductDetail)}' is green first before testing the GUID constraint.");
        }

        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/detail/of/product/not-a-valid-guid");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [MonitoredTheory]
    [InlineData("Food", 15)]
    [InlineData("Pets", 21)]
    public async Task _05_Get_CategoriesProducts_WithCategoryAndProductsPerPage_ShouldMapToProductOverview(
        string categoryCode, int productsPerPage)
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/categories/{categoryCode}/products/{productsPerPage}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var document = await HtmlHelpers.GetDocumentAsync(response);
        Assert.Contains($"*{categoryCode}*", document.Body?.TextContent ?? string.Empty);
        Assert.Contains($"*{productsPerPage}*", document.Body?.TextContent ?? string.Empty);
    }

    [MonitoredFact]
    public async Task _06_Get_CategoriesProducts_WithoutProductsPerPage_ShouldMapToProductOverviewWithDefaultValue()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/categories/Pets/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var document = await HtmlHelpers.GetDocumentAsync(response);
        Assert.Contains("*Pets*", document.Body?.TextContent ?? string.Empty);
        Assert.Contains("*10*", document.Body?.TextContent ?? string.Empty);
    }

    [MonitoredTheory]
    [InlineData("/categories/123/products/5")]
    [InlineData("/categories/Pets/products/five")]
    public async Task _07_Get_CategoriesProducts_ViolatingRouteConstraints_ShouldReturnNotFound(string url)
    {
        try
        {
            await _06_Get_CategoriesProducts_WithoutProductsPerPage_ShouldMapToProductOverviewWithDefaultValue();
        }
        catch (Exception)
        {
            Assert.Fail($"Make sure test '{nameof(_06_Get_CategoriesProducts_WithoutProductsPerPage_ShouldMapToProductOverviewWithDefaultValue)}' is green first before testing the route constraints.");
        }

        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [MonitoredFact]
    public void _08_Program_ShouldDefineAtMostThreeRoutes()
    {
        string programContent = Solution.Current.GetFileContent(@"Route98.Web\Program.cs");
        int routeCount = Regex.Matches(programContent, @"\.Map(Default)?ControllerRoute\s*\(").Count;

        Assert.InRange(routeCount, 1, 3);
    }

    [MonitoredFact]
    public async Task _09_Layout_ShouldUseTagHelpersAndRenderNavigationLinksCorrectly()
    {
        string layoutContent = Solution.Current.GetFileContent(@"Route98.Web\Views\Shared\_Layout.cshtml");
        Assert.DoesNotContain("href=\"/\"", layoutContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("href=\"/Privacy/of/Home\"", layoutContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("asp-controller=", layoutContent);
        Assert.Contains("asp-action=", layoutContent);

        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        var response = await client.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var document = await HtmlHelpers.GetDocumentAsync(response);

        var brandLink = document.QuerySelector("a.navbar-brand") as IHtmlAnchorElement;
        Assert.NotNull(brandLink);
        Assert.Equal("/", brandLink.GetAttribute("href"));

        var navLinks = document.QuerySelectorAll("a.nav-link").OfType<IHtmlAnchorElement>().ToList();
        Assert.Equal(2, navLinks.Count);

        var homeLink = navLinks.FirstOrDefault(a => a.TextContent.Trim() == "Home");
        Assert.NotNull(homeLink);
        Assert.Equal("/", homeLink.GetAttribute("href"));

        var privacyLink = navLinks.FirstOrDefault(a => a.TextContent.Trim() == "Privacy");
        Assert.NotNull(privacyLink);
        Assert.Equal("/Privacy/of/Home", privacyLink.GetAttribute("href"), ignoreCase: true);
    }

    [MonitoredFact]
    public async Task _10_HomeIndex_ShouldUseTagHelpersAndRenderProductLinksCorrectly()
    {
        string indexContent = Solution.Current.GetFileContent(@"Route98.Web\Views\Home\Index.cshtml");
        Assert.DoesNotContain("href=", indexContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("asp-controller=", indexContent);
        Assert.Contains("asp-action=", indexContent);
        Assert.Contains("asp-route-id=", indexContent);
        Assert.Contains("asp-route-categoryCode=", indexContent);
        Assert.Contains("asp-route-productsPerPage=", indexContent);

        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        var response = await client.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var document = await HtmlHelpers.GetDocumentAsync(response);
        var links = document.QuerySelectorAll("ul.list-group li.list-group-item a")
            .OfType<IHtmlAnchorElement>()
            .ToList();

        Assert.Equal(3, links.Count);
        Assert.Equal("/Detail/of/Product/04efde7c-5274-424f-a27d-e64b3367d43d",
            links[0].GetAttribute("href"), ignoreCase: true);
        Assert.Equal("/categories/Pets/products/5",
            links[1].GetAttribute("href"), ignoreCase: true);
        Assert.Equal("/categories/Food/products",
            links[2].GetAttribute("href"), ignoreCase: true);
    }
}
