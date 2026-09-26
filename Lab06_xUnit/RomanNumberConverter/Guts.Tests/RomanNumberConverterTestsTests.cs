using System.Reflection;
using System.Text;
using Guts.Client.Core.TestTools;
using Guts.Client.XUnit;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RomanNumberConverterApp.Ui.Tests.Models;

namespace Guts.Tests;

[ExerciseTestClass("dotNet2", "xUnit", "romanNumberConverter", @"RomanNumberConverterApp.Ui.Tests\Models\RomanNumberConverterTests.cs")]
public class RomanNumberConverterTestsTests
{
    private const string ValueShouldBeInRangeMethodName = "Convert_ValueIsNotBetweenOneAnd3999_ShouldThrowArgumentException";
    private const string ValidValueShouldConvertCorrectlyMethodName = "Convert_ValidValue_ShouldReturnRomanNumberEquivalent";
    private readonly Type _testClassType = typeof(RomanNumberConverterTests);
    private readonly string _testClassContent = Solution.Current.GetFileContent(@"RomanNumberConverterApp.Ui.Tests\Models\RomanNumberConverterTests.cs");

    [MonitoredFact]
    public void ShouldHaveATestThatChecksIfTheValueIsInRange()
    {
        var testMethod = GetTestMethod(ValueShouldBeInRangeMethodName, 1);
        var inlineData = GetInlineData(testMethod);

        Assert.True(inlineData.Count >= 2, "The method should have at least 2 test cases.");
        Assert.All(inlineData, testCase => Assert.Single(testCase.Data));
        Assert.All(inlineData, testCase =>
        {
            var value = Assert.IsType<int>(Assert.Single(testCase.Data));
            Assert.False(value is >= 1 and <= 3999, "The method should only test values outside the 1-3999 range.");
        });
        Assert.Contains(inlineData, testCase => (int)(Assert.Single(testCase.Data) ?? -1) == 0);
        Assert.Contains(inlineData, testCase => (int)(Assert.Single(testCase.Data) ?? -1) == 4000);

        var methodBody = GetMethodBodyWithoutComments(ValueShouldBeInRangeMethodName);
        Assert.Contains(".Convert(", methodBody);
        Assert.Contains("Assert.Throws<ArgumentException>", methodBody);
        Assert.Contains("exception.Message", methodBody);
        Assert.Contains("1-3999", methodBody);
    }

    [MonitoredFact]
    public void TheTestThatChecksIfTheValueIsInRangeShouldPass()
    {
        AssertTestMethodPasses(GetTestMethod(ValueShouldBeInRangeMethodName, 1));
    }

    [MonitoredFact]
    public void TheTestThatChecksIfTheValueIsInRangeShouldPassForCorrectExpectations()
    {
        var testMethod = GetTestMethod(ValueShouldBeInRangeMethodName, 1);
        AssertTestMethodPasses(testMethod, 0);
        AssertTestMethodPasses(testMethod, 4000);
        AssertTestMethodPasses(testMethod, Random.Shared.Next(4001, int.MaxValue));
        AssertTestMethodPasses(testMethod, -Random.Shared.Next(1, int.MaxValue));
    }

    [MonitoredFact]
    public void TheTestThatChecksIfTheValueIsInRangeShouldFailForWrongExpectation()
    {
        var testMethod = GetTestMethod(ValueShouldBeInRangeMethodName, 1);

        foreach (var validValue in GetSomeValidValues(5))
        {
            AssertTestMethodFails(testMethod, validValue);
        }
    }

    [MonitoredFact]
    public void ShouldHaveATestThatChecksConversionOfValidNumbers()
    {
        var testMethod = GetTestMethod(ValidValueShouldConvertCorrectlyMethodName, 2);
        var inlineData = GetInlineData(testMethod);

        Assert.True(inlineData.Count >= 4, "The method should have at least 4 test cases.");
        Assert.All(inlineData, testCase => Assert.Equal(2, testCase.Data.Length));
        Assert.All(inlineData, testCase =>
        {
            var value = Assert.IsType<int>(testCase.Data[0]);
            var expected = Assert.IsType<string>(testCase.Data[1]);
            Assert.InRange(value, 1, 3999);
            Assert.NotEmpty(expected);
            Assert.All(expected, character => Assert.Contains(character, "IVXLCDM"));
        });

        var methodBody = GetMethodBodyWithoutComments(ValidValueShouldConvertCorrectlyMethodName);
        Assert.Contains(".Convert(", methodBody);
        Assert.Contains("Assert.Equal(", methodBody);
    }

    [MonitoredFact]
    public void TheTestThatChecksConversionOfValidNumbersShouldPass()
    {
        AssertTestMethodPasses(GetTestMethod(ValidValueShouldConvertCorrectlyMethodName, 2));
    }

    [MonitoredFact]
    public void TheTestThatChecksConversionOfValidNumbersShouldPassForCorrectExpectations()
    {
        var testMethod = GetTestMethod(ValidValueShouldConvertCorrectlyMethodName, 2);
        foreach (var validValue in GetSomeValidValues(100))
        {
            AssertTestMethodPasses(testMethod, validValue, ToRomanNumber(validValue));
        }
    }

    [MonitoredFact]
    public void TheTestThatChecksConversionOfValidNumbersShouldFailForWrongExpectation()
    {
        var testMethod = GetTestMethod(ValidValueShouldConvertCorrectlyMethodName, 2);
        foreach (var (value, expected) in new[] { (1, "II"), (3, "II"), (4, "IIII"), (9, "VIIII"), (10, "VV"), (40, "XXXX"), (90, "LXXXX"), (400, "CCCC"), (900, "DCCCC"), (1000, "DD") })
        {
            AssertTestMethodFails(testMethod, value, expected);
        }
    }

    private MethodInfo GetTestMethod(string methodName, int expectedParameterCount)
    {
        var testMethod = _testClassType.GetMethod(methodName);
        Assert.False(testMethod is null, $"The test method '{methodName}' was not found.");
        Assert.Equal(expectedParameterCount, testMethod.GetParameters().Length);
        Assert.True(expectedParameterCount == testMethod.GetParameters().Length, $"The test method '{methodName}' should have {expectedParameterCount} parameters.");
        Assert.False(testMethod.GetCustomAttribute<TheoryAttribute>() is null, "The test method should be decorated with an attribute that indicates it is an xUnit test for multiple inputs");
        return testMethod;
    }

    private static List<InlineDataAttribute> GetInlineData(MethodInfo testMethod) => testMethod.GetCustomAttributes<InlineDataAttribute>().ToList();

    private string GetMethodBodyWithoutComments(string methodName)
    {
        var method = CSharpSyntaxTree.ParseText(_testClassContent).GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(declaration => declaration.Identifier.ValueText == methodName);

        return method is null ? string.Empty : string.Join(Environment.NewLine, method.Body!.Statements);
    }

    private static IList<int> GetSomeValidValues(int numberOfValues) => Enumerable.Range(0, numberOfValues).Select(_ => Random.Shared.Next(1, 4000)).ToList();

    private static void AssertTestMethodFails(MethodInfo testMethod, params object[] parameters)
    {
        Assert.NotNull(Record.Exception(() => testMethod.Invoke(new RomanNumberConverterTests(), parameters)));
    }

    private static void AssertTestMethodPasses(MethodInfo testMethod)
    {
        foreach (InlineDataAttribute inlineData in GetInlineData(testMethod))
        {
            AssertTestMethodPasses(testMethod, inlineData.Data);
        }
    }

    private static void AssertTestMethodPasses(MethodInfo testMethod, params object?[] parameters)
    {
        Assert.Null(Record.Exception(() => testMethod.Invoke(new RomanNumberConverterTests(), parameters)));
    }

    private static string ToRomanNumber(int value)
    {
        var numerals = new[] { (1000, "M"), (900, "CM"), (500, "D"), (400, "CD"), (100, "C"), (90, "XC"), (50, "L"), (40, "XL"), (10, "X"), (9, "IX"), (5, "V"), (4, "IV"), (1, "I") };
        var result = new StringBuilder();
        foreach (var (number, numeral) in numerals)
        {
            while (value >= number)
            {
                result.Append(numeral);
                value -= number;
            }
        }

        return result.ToString();
    }
}
