using Guts.Client.Core.TestTools;
using Guts.Client.XUnit;
using Xunit;

namespace SmurfApp.Web.Tests;

[ExerciseTestClass("dotnet2", "8-ViewsAndForms", "SmurfApp")]
public class HomeTechnicalTests
{
    private readonly string _indexView = string.Empty;

    public HomeTechnicalTests()
    {
        try
        {
            _indexView = Solution.Current.GetFileContent("SmurfApp.Web/Views/Home/Index.cshtml");
        }
        catch
        {
            // Ignore
        }
    }

    [MonitoredFact]
    public void IndexView_ShouldUsePartialViewForTheSmurfCards()
    {
        Assert.NotEmpty(_indexView);
        Assert.Contains("<partial", _indexView, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("class=\"card", _indexView, StringComparison.OrdinalIgnoreCase);
    }

    [MonitoredFact]
    public void IndexView_ShouldUseALayout()
    {
        Assert.NotEmpty(_indexView);
        Assert.DoesNotContain("<html", _indexView, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("<header", _indexView, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("<footer", _indexView, StringComparison.OrdinalIgnoreCase);
    }

    [MonitoredFact]
    public void IndexView_ShouldUseAModelDirective()
    {
        Assert.NotEmpty(_indexView);
        Assert.Contains("@model", _indexView);
        Assert.Contains("IList<Smurf>", _indexView);
    }

    [MonitoredFact]
    public void IndexView_ShouldUseTagHelpersForTheAddNewButton()
    {
        Assert.NotEmpty(_indexView);
        Assert.DoesNotContain("href", _indexView, StringComparison.OrdinalIgnoreCase);
    }
}
