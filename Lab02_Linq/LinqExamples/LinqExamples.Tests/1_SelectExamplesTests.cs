using Guts.Client.NUnit;
using LinqExamples.Models;

namespace LinqExamples.Tests;

[ExerciseTestFixture("dotNet2", "2-Linq", "LinqExamples", @"LinqExamples\SelectExamples.cs")]
public class SelectExamplesTests
{
    private SelectExamples _examples = null!;

    [SetUp]
    public void Setup()
    {
        _examples = new SelectExamples();
    }

    [MonitoredTest]
    public void GetLengthOfWords_AllNonNullWords_ShouldReturnLengthOfEachWord()
    {
        string guidWord = Guid.NewGuid().ToString();
        var words = new List<string?> { "Hello", "World", "!", guidWord };
        var expected = new List<int> { 5, 5, 1, guidWord.Length };

        IList<int> actual = _examples.GetLengthOfWords(words);

        Assert.That(actual, Is.EquivalentTo(expected));
    }

    [MonitoredTest]
    public void GetLengthOfWords_SomeWordsAreNull_ShouldReturnZeroLengthForNullWords()
    {
        var words = new List<string?> { "Hello", null, "World", null, "!" };
        var expected = new List<int> { 5, 0, 5, 0, 1 };

        IList<int> actual = _examples.GetLengthOfWords(words);

        Assert.That(actual, Is.EquivalentTo(expected));
    }

    [MonitoredTest]
    public void ConvertAnglesToAngleInfos_ShouldConvertEachAngleToAnAngleInfoWithAngleCosinusAndSinus()
    {
        //Arrange
        var angles = new List<double> { 10d, 45d, 90d };
        double randomAngle = new Random().NextDouble() * 360;
        angles.Add(randomAngle);
        var expected = new List<AngleInfo>
        {
            new() {Angle = 10, Cosinus = Math.Cos(10 * Math.PI / 180), Sinus = Math.Sin(10 * Math.PI / 180)},
            new() {Angle = 45, Cosinus = Math.Cos(45 * Math.PI / 180), Sinus = Math.Sin(45 * Math.PI / 180)},
            new() {Angle = 90, Cosinus = Math.Cos(90 * Math.PI / 180), Sinus = Math.Sin(90 * Math.PI / 180)},
            new() {Angle = randomAngle, Cosinus = Math.Cos(randomAngle * Math.PI / 180), Sinus = Math.Sin(randomAngle * Math.PI / 180)}
        };

        //Act
        IList<AngleInfo> actual = _examples.ConvertAnglesToAngleInfos(angles);

        //Assert
        Assert.That(actual, Is.EquivalentTo(expected));
    }
}