using System.Reflection;
using Guts.Client.XUnit;

namespace Guts.Tests;

[ExerciseTestClass("dotNet2", "xUnit", "BankApp")]
public class BankUiTestsProjectTests
{
    [MonitoredFact]
    public void ShouldContainABankUiTestsProject()
    {
        Assert.True(TestProjectLocator.TestProjectExists,
            "There is no 'Bank.UI.Tests' project in the solution yet. Add a class library project with that name.");
    }

    [MonitoredFact]
    public void TheBankUiTestsProjectShouldContainACheckingAccountTestsClass()
    {
        Assert.NotNull(GetTestClass("CheckingAccountTests"));
    }

    [MonitoredFact]
    public void TheBankUiTestsProjectShouldContainASavingsAccountTestsClass()
    {
        Assert.NotNull(GetTestClass("SavingsAccountTests"));
    }

    internal static Type? GetTestClass(string simpleClassName) =>
        TestProjectLocator.TestAssembly.GetTypes().FirstOrDefault(type => type.Name == simpleClassName);

    internal static int CountTestCases(Type testClass)
    {
        int count = 0;
        foreach (MethodInfo method in testClass.GetMethods(BindingFlags.Public | BindingFlags.Instance))
        {
            var attributeNames = method.GetCustomAttributes().Select(a => a.GetType().Name).ToList();
            if (attributeNames.Contains("FactAttribute"))
            {
                count++;
            }
            else if (attributeNames.Contains("TheoryAttribute"))
            {
                int inlineDataCount = method.GetCustomAttributes().Count(a => a.GetType().Name == "InlineDataAttribute");
                count += Math.Max(inlineDataCount, 1);
            }
        }

        return count;
    }
}
