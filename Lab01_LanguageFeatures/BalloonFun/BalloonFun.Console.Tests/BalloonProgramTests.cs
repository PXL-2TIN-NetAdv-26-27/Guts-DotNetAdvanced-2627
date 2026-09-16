using Guts.Client.Core.TestTools;
using Guts.Client.XUnit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace BalloonFun.Console.Tests;

[ExerciseTestClass("dotNet2", "1-LanguageFeatures", "BalloonFun", @"BalloonFun.Console\BalloonProgram.cs")]
public class BalloonProgramTests
{
    [MonitoredFact]
    public void RunShouldWriteFiveBalloonsToOutputThenThePoppedBalloon()
    {
        var output = new RecordingOutputWriter();
        var program = new BalloonProgram(output);

        program.Run();

        Assert.Equal(6, output.Messages.Count);
        Assert.All(output.Messages.Take(5), message =>
        {
            Assert.StartsWith("A balloon of size '", message);
            Assert.Contains("and color '", message);
        });
        Assert.Contains("Popped balloon", output.Messages[5]);
    }

    [MonitoredFact]
    public void RunShouldUseNullCoalescingOperatorForBalloonName()
    {
        string content = Solution.Current.GetFileContent(@"BalloonFun.Console\BalloonProgram.cs");
        var syntaxTree = CSharpSyntaxTree.ParseText(content);
        var root = syntaxTree.GetRoot();

        var runMethod = root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(m => m.Identifier.ValueText == nameof(BalloonProgram.Run));

        Assert.NotNull(runMethod);

        var coalesceExpressions = runMethod.DescendantNodes()
            .OfType<BinaryExpressionSyntax>()
            .Where(b => b.IsKind(SyntaxKind.CoalesceExpression))
            .ToList();

        Assert.True(coalesceExpressions.Count > 0,
            "The Run method should use the null-coalescing operator (??) to handle a missing balloon name.");

        bool usesFallbackString = coalesceExpressions.Any(c => c.Right.ToString().Contains("\""));
        Assert.True(usesFallbackString,
            "The null-coalescing expression should default to a string like 'Anonymous' when the balloon name is null.");
    }

    private sealed class RecordingOutputWriter : IOutputWriter
    {
        public List<string> Messages { get; } = [];

        public void Write(string message) => Messages.Add(message);
    }
}
