using System.Drawing;
using System.Reflection;
using Guts.Client.XUnit;
using Xunit;

namespace BalloonFun.Console.Tests;

[ExerciseTestClass("dotNet2", "1-LanguageFeatures", "BalloonFun", @"BalloonFun.Console\Balloon.cs")]
public class BalloonTests
{
    [MonitoredFact]
    public void ShouldBeAValueType()
    {
        Assert.True(typeof(Balloon).IsValueType, "Balloon must be a struct (value type).");
    }

    [MonitoredFact]
    public void PropertiesColorAndSizeShouldOnlyBeAssignableInConstructorOrDeclaration()
    {
        Type balloonType = typeof(Balloon);

        PropertyInfo? colorProperty = balloonType.GetProperty(nameof(Balloon.Color));
        Assert.NotNull(colorProperty);
        Assert.True(colorProperty.CanRead, "Color property should have a getter.");
        Assert.True(colorProperty.SetMethod == null || !colorProperty.SetMethod.IsPublic,
            "Color property should only be assignable in constructor or declaration.");

        PropertyInfo? sizeProperty = balloonType.GetProperty(nameof(Balloon.Size));
        Assert.NotNull(sizeProperty);
        Assert.True(sizeProperty.CanRead, "Size property should have a getter.");
        Assert.True(sizeProperty.SetMethod == null || !sizeProperty.SetMethod.IsPublic,
            "Size property should only be assignable in constructor or declaration.");
    }

    [MonitoredFact]
    public void ConstructorShouldInitializeColorAndSizeAndSetNameToNull()
    {
        var balloon = new Balloon(Color.Red, 42);

        Assert.Equal(Color.Red, balloon.Color);
        Assert.Equal(42, balloon.Size);
        Assert.Null(balloon.Name);
    }

    [MonitoredFact]
    public void NamePropertyCanOnlyBeAssignedByBaptize()
    {
        PropertyInfo? nameProperty = typeof(Balloon).GetProperty(nameof(Balloon.Name));
        Assert.True(nameProperty is not null, "Cannot find a `Name` property.");

        MethodInfo? setMethod = nameProperty.SetMethod;
        Assert.True(setMethod != null && !setMethod.IsPublic,
            "The `Name` property should have a non-public setter.");

        var balloon = new Balloon(Color.Blue, 15);
        balloon.Baptize("Christian Baloon");

        Assert.Equal("Christian Baloon", balloon.Name);
    }
}
