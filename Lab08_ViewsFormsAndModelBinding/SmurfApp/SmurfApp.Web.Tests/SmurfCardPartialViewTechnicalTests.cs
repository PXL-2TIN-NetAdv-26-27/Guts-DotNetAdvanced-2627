using Guts.Client.Core.TestTools;
using Guts.Client.XUnit;
using SmurfApp.Domain;
using Xunit;

namespace SmurfApp.Web.Tests;

[ExerciseTestClass("dotnet2", "8-ViewsAndForms", "SmurfApp")]
public class SmurfCardPartialViewTechnicalTests
{
    private readonly string _partialView = string.Empty;

    public SmurfCardPartialViewTechnicalTests()
    {
        try
        {
            string webProjectPath = Path.Combine(Solution.Current.Path, "SmurfApp.Web");
            string? filePath = Directory.GetFiles(webProjectPath, "_SmurfCard.cshtml", SearchOption.AllDirectories).FirstOrDefault();
            if (filePath is not null)
            {
                _partialView = File.ReadAllText(filePath);
            }
        }
        catch
        {
            // Ignore
        }
    }

    [MonitoredFact]
    public void PartialView_ShouldUseAModelDirective()
    {
        Assert.NotEmpty(_partialView);
        Assert.Contains("@model Smurf", _partialView);
    }

    [MonitoredFact]
    public void PartialView_ShouldUseTheModel()
    {
        Assert.NotEmpty(_partialView);
        Assert.Contains("@Model.", _partialView);
        string[] smurfProperties = [nameof(Smurf.Name), nameof(Smurf.Age), nameof(Smurf.ImageUrl), nameof(Smurf.Category), nameof(Smurf.Id)]; 
        foreach (string property in smurfProperties)
        {
            Assert.Contains($".{property}", _partialView);
        }
    }

    [MonitoredFact]
    public void PartialView_ShouldUseTagHelpersForTheDetailsButton()
    {
        Assert.NotEmpty(_partialView);
        Assert.DoesNotContain("href", _partialView, StringComparison.OrdinalIgnoreCase);
    }
}
