using Guts.Client.XUnit;

namespace Guts.Tests;

[ExerciseTestClass("dotNet2", "xUnit", "BankApp")]
public class SavingsAccountTestsTests
{
    private const int MinimumNumberOfTestCases = 8;
    private const string TestClassFullNamePrefix = "Bank.UI.Tests.SavingsAccountTests.";

    [MonitoredFact]
    public void ShouldHaveEnoughTestCasesForSavingsAccount()
    {
        var testClass = BankUiTestsProjectTests.GetTestClass("SavingsAccountTests");
        Assert.False(testClass is null, "The 'SavingsAccountTests' class was not found in 'Bank.UI.Tests'.");

        int testCaseCount = BankUiTestsProjectTests.CountTestCases(testClass!);
        Assert.True(testCaseCount >= MinimumNumberOfTestCases,
            $"'SavingsAccountTests' should contain at least {MinimumNumberOfTestCases} test cases (Facts + Theory data rows), found {testCaseCount}.");
    }

    [MonitoredFact]
    public void AllSavingsAccountTestsShouldPass()
    {
        var results = TrxParser.ParseResults(TestProjectLocator.TrxResultsPath)
            .Where(result => result.TestName.StartsWith(TestClassFullNamePrefix, StringComparison.Ordinal))
            .ToList();

        Assert.True(results.Count >= MinimumNumberOfTestCases,
            $"Expected at least {MinimumNumberOfTestCases} executed tests for 'SavingsAccountTests', found {results.Count}.");

        var failing = results.Where(result => !result.Passed).Select(result => result.TestName).ToList();
        Assert.True(failing.Count == 0, $"The following 'SavingsAccountTests' tests did not pass: {string.Join(", ", failing)}");
    }
}
