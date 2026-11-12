using Guts.Client.Core.TestTools;
using Guts.Client.XUnit;
using Xunit;

namespace SmurfApp.Web.Tests;

[ExerciseTestClass("dotnet2", "8-ViewsAndForms", "SmurfApp",
    @"SmurfApp.Web\Views\Shared\_TopNavBar.cshtml")]
public class TopNavBarPartialViewTechnicalTests
{
    private readonly string _layoutView = string.Empty;
    private readonly string _navBarView = string.Empty;

    public TopNavBarPartialViewTechnicalTests()
    {
        try
        {
            _layoutView = Solution.Current.GetFileContent("SmurfApp.Web/Views/Shared/_Layout.cshtml");
            _navBarView = Solution.Current.GetFileContent("SmurfApp.Web/Views/Shared/_TopNavBar.cshtml");
        }
        catch
        {
            // Ignore
        }
    }

    [MonitoredFact]
    public void LayoutView_ShouldUsePartialViewForTheTopNavigation()
    {
        Assert.Contains("<partial", _layoutView, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("_TopNavBar", _layoutView, StringComparison.OrdinalIgnoreCase);
    }

    [MonitoredFact]
    public void PartialView_ShouldUseTagHelpersForTheLinks()
    {
        Assert.DoesNotContain("href", _navBarView, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("asp-", _navBarView, StringComparison.OrdinalIgnoreCase);
    }

    [MonitoredFact]
    public void PartialView_ShouldUseTagHelperForTheSmurfCountViewComponent()
    {
        Assert.Contains("<vc:smurf-count", _navBarView, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("class=\"badge", _navBarView, StringComparison.OrdinalIgnoreCase);
    }
}
