using Guts.Client.XUnit;

namespace Guts.Tests;

[ExerciseTestClass("dotNet2", "xUnit", "BankApp")]
public class CheckingAccountTestsTests
{
    private const int MinimumNumberOfTestCases = 8;
    private const string TestClassFullNamePrefix = "Bank.UI.Tests.CheckingAccountTests.";

    [MonitoredFact]
    public void ShouldHaveEnoughTestCasesForCheckingAccount()
    {
        var testClass = BankUiTestsProjectTests.GetTestClass("CheckingAccountTests");
        Assert.False(testClass is null, "The 'CheckingAccountTests' class was not found in 'Bank.UI.Tests'.");

        int testCaseCount = BankUiTestsProjectTests.CountTestCases(testClass!);
        Assert.True(testCaseCount >= MinimumNumberOfTestCases,
            $"'CheckingAccountTests' should contain at least {MinimumNumberOfTestCases} test cases (Facts + Theory data rows), found {testCaseCount}.");
    }

    [MonitoredFact]
    public void AllCheckingAccountTestsShouldPass()
    {
        var results = TrxParser.ParseResults(TestProjectLocator.TrxResultsPath)
            .Where(result => result.TestName.StartsWith(TestClassFullNamePrefix, StringComparison.Ordinal))
            .ToList();

        Assert.True(results.Count >= MinimumNumberOfTestCases,
            $"Expected at least {MinimumNumberOfTestCases} executed tests for 'CheckingAccountTests', found {results.Count}.");

        var failing = results.Where(result => !result.Passed).Select(result => result.TestName).ToList();
        Assert.True(failing.Count == 0, $"The following 'CheckingAccountTests' tests did not pass: {string.Join(", ", failing)}");
    }
}
