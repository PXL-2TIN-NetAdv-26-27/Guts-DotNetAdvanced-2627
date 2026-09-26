using Guts.Client.XUnit;

namespace Guts.Tests;

[ExerciseTestClass("dotNet2", "xUnit", "BankApp")]
public class TestCoverageTests
{
    private const int MinimumTotalNumberOfTestCases = 20;

    // Baseline measured on the reference solution: BankAccount ~86%, CheckingAccount 100%, SavingsAccount 100%.
    // Thresholds are set a bit below that baseline to allow for slightly different, still-thorough implementations.
    private const double MinimumBankAccountLineCoverage = 0.80;
    private const double MinimumCheckingAccountLineCoverage = 0.80;
    private const double MinimumSavingsAccountLineCoverage = 0.80;

    [MonitoredFact]
    public void ShouldHaveEnoughTestCasesInTotal()
    {
        var results = TrxParser.ParseResults(TestProjectLocator.TrxResultsPath);
        Assert.True(results.Count >= MinimumTotalNumberOfTestCases,
            $"Expected at least {MinimumTotalNumberOfTestCases} executed tests in 'Bank.UI.Tests', found {results.Count}.");
    }

    [MonitoredFact]
    public void BankAccountShouldBeSufficientlyCovered()
    {
        AssertClassCoverageAtLeast("Bank.UI.BankAccount", MinimumBankAccountLineCoverage);
    }

    [MonitoredFact]
    public void CheckingAccountShouldBeSufficientlyCovered()
    {
        AssertClassCoverageAtLeast("Bank.UI.CheckingAccount", MinimumCheckingAccountLineCoverage);
    }

    [MonitoredFact]
    public void SavingsAccountShouldBeSufficientlyCovered()
    {
        AssertClassCoverageAtLeast("Bank.UI.SavingsAccount", MinimumSavingsAccountLineCoverage);
    }

    private static void AssertClassCoverageAtLeast(string className, double minimumLineRate)
    {
        double? lineRate = CoverageParser.GetClassLineRate(TestProjectLocator.CoberturaPath, className);

        Assert.False(lineRate is null, $"No coverage data was found for class '{className}'. Make sure it is covered by tests in 'Bank.UI.Tests'.");
        Assert.True(lineRate >= minimumLineRate,
            $"Line coverage for '{className}' is {lineRate:P0}, which is below the required {minimumLineRate:P0}.");
    }
}
