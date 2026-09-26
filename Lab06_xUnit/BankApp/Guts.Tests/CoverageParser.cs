using System.Xml.Linq;

namespace Guts.Tests;

/// <summary>
/// Reads per-class line coverage from a cobertura.xml report produced by 'dotnet-coverage'.
/// </summary>
internal static class CoverageParser
{
    public static double? GetClassLineRate(string coberturaPath, string classNameSuffix)
    {
        var document = XDocument.Load(coberturaPath);

        var matchingClass = document.Descendants("class")
            .FirstOrDefault(c => ((string?)c.Attribute("name"))?.EndsWith(classNameSuffix, StringComparison.Ordinal) == true);

        string? lineRate = (string?)matchingClass?.Attribute("line-rate");
        return lineRate is not null ? double.Parse(lineRate, System.Globalization.CultureInfo.InvariantCulture) : null;
    }
}
