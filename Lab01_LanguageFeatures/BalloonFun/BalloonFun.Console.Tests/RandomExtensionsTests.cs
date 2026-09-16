using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using BalloonFun.Console;
using Guts.Client.XUnit;
using Xunit;

namespace BalloonFun.Console.Tests;

[ExerciseTestClass("dotNet2", "1-LanguageFeatures", "BalloonFun", @"BalloonFun.Console\RandomExtensions.cs")]
public class RandomExtensionsTests
{
    [MonitoredFact]
    public void ShouldBeAPublicStaticClass()
    {
        Type extensionsType = typeof(RandomExtensions);
        Assert.True(extensionsType.IsAbstract && extensionsType.IsSealed, "RandomExtensions must be a static class.");
        Assert.True(extensionsType.IsPublic, "RandomExtensions must be public.");
    }

    [MonitoredFact]
    public void NextBalloonShouldBeAnExtensionMethodOfRandom()
    {
        MethodInfo method = GetExtensionMethod(nameof(RandomExtensions.NextBalloon));
        ParameterInfo[] parameters = method.GetParameters();

        Assert.Equal(2, parameters.Length);
        Assert.Equal(typeof(Random), parameters[0].ParameterType);
        Assert.Equal(typeof(int), parameters[1].ParameterType);
        Assert.Equal(typeof(Balloon), method.ReturnType);
    }

    [MonitoredFact]
    public void NextBalloonFromArrayShouldBeAnExtensionMethodOfRandom()
    {
        MethodInfo method = GetExtensionMethod(nameof(RandomExtensions.NextBalloonFromArray));
        ParameterInfo[] parameters = method.GetParameters();

        Assert.Equal(2, parameters.Length);
        Assert.Equal(typeof(Random), parameters[0].ParameterType);
        Assert.Equal(typeof(Balloon[]), parameters[1].ParameterType);
        Assert.Equal(typeof(Balloon), method.ReturnType);
    }

    [MonitoredFact]
    public void NextBalloonShouldReturnBalloonWithSizeInRangeAndValidColor()
    {
        MethodInfo method = GetExtensionMethod(nameof(RandomExtensions.NextBalloon));
        var random = new Random(1234);
        var generatedSizes = new HashSet<int>();
        var generatedColors = new HashSet<Color>();

        for (int i = 0; i < 100; i++)
        {
            var balloon = (Balloon)method.Invoke(null, [random, 20])!;

            Assert.InRange(balloon.Size, 1, 20);
            Assert.InRange(balloon.Color.R, 0, 255);
            Assert.InRange(balloon.Color.G, 0, 255);
            Assert.InRange(balloon.Color.B, 0, 255);

            generatedSizes.Add(balloon.Size);
            generatedColors.Add(balloon.Color);
        }

        Assert.True(generatedSizes.Count > 1, "NextBalloon should generate balloons with varying sizes.");
        Assert.True(generatedColors.Count > 1, "NextBalloon should generate balloons with varying colors.");
    }

    [MonitoredFact]
    public void NextBalloonFromArrayShouldReturnAnItemFromGivenArray()
    {
        MethodInfo method = GetExtensionMethod(nameof(RandomExtensions.NextBalloonFromArray));
        var balloons = new[]
        {
            new Balloon(Color.Red, 5),
            new Balloon(Color.Green, 10),
            new Balloon(Color.Blue, 15)
        };
        var random = new Random(5678);
        var selectedBalloons = new HashSet<Balloon>();

        for (int i = 0; i < 100; i++)
        {
            var selected = (Balloon)method.Invoke(null, [random, balloons])!;
            Assert.Contains(selected, balloons);
            selectedBalloons.Add(selected);
        }

        Assert.Equal(balloons.Length, selectedBalloons.Count);
    }

    private static MethodInfo GetExtensionMethod(string name)
    {
        MethodInfo? method = typeof(RandomExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == name && m.IsDefined(typeof(ExtensionAttribute), inherit: false));

        Assert.True(method != null, $"The extension method '{name}' was not found on '{nameof(RandomExtensions)}'.");
        return method!;
    }
}
