using System.Xml.Linq;

namespace Guts.Tests;

internal sealed record TestResult(string TestName, string Outcome)
{
    public bool Passed => Outcome.Equals("Passed", StringComparison.OrdinalIgnoreCase);
}

/// <summary>
/// Reads per-test outcomes from a Visual Studio .trx test results file produced by 'dotnet test'.
/// </summary>
internal static class TrxParser
{
    private static readonly XNamespace Ns = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";

    public static IReadOnlyList<TestResult> ParseResults(string trxPath)
    {
        var document = XDocument.Load(trxPath);

        return document.Descendants(Ns + "UnitTestResult")
            .Select(result =>
            {
                string name = (string?)result.Attribute("testName") ?? "Unknown";
                string outcome = (string?)result.Attribute("outcome") ?? "Unknown";
                return new TestResult(name, outcome);
            })
            .ToList();
    }
}
