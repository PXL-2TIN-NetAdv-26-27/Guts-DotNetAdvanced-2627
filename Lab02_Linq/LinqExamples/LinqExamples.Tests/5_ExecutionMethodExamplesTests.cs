using Guts.Client.NUnit;
using LinqExamples.Models;

namespace LinqExamples.Tests;

[ExerciseTestFixture("dotNet2", "2-Linq", "LinqExamples", @"LinqExamples\ExecutionMethodExamples.cs")]
public class ExecutionMethodExamplesTests
{
    private ExecutionMethodExamples _examples = null!;

    [SetUp]
    public void Setup()
    {
        _examples = new ExecutionMethodExamples();
    }

    [MonitoredTest]
    public void GetFirstPersonBornBefore2000_HasPersonsBornBefore2000_ShouldReturnFirstPersonBornBefore2000()
    {
        // Arrange
        var persons = new List<Person>
        {
            new() { Name = "John", BirthDate = new DateTime(2001, 5, 15) },
            new() { Name = "Alice", BirthDate = new DateTime(1995, 3, 10) },
            new() { Name = "Bob", BirthDate = new DateTime(1998, 12, 25) },
            new() { Name = "Carol", BirthDate = new DateTime(1985, 7, 8) }
        };
        var expected = persons[1]; // Alice, born in 1995

        // Act
        Person? actual = _examples.GetFirstPersonBornBefore2000(persons);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [MonitoredTest]
    public void GetFirstPersonBornBefore2000_NoPersonsBornBefore2000_ShouldReturnNull()
    {
        // Arrange
        var persons = new List<Person>
        {
            new() { Name = "John", BirthDate = new DateTime(2001, 5, 15) },
            new() { Name = "Alice", BirthDate = new DateTime(2005, 3, 10) },
            new() { Name = "Bob", BirthDate = new DateTime(2010, 12, 25) }
        };

        // Act
        Person? actual = _examples.GetFirstPersonBornBefore2000(persons);

        // Assert
        Assert.That(actual, Is.Null);
    }

    [MonitoredTest]
    public void GetTheOnlyOneWithANameLongerThan20Characters_ExactlyOnePersonWithLongName_ShouldReturnThatPerson()
    {
        // Arrange
        string longName = "ThisIsAVeryLongNameWithMoreThan20Characters";
        var persons = new List<Person>
        {
            new() { Name = "John", BirthDate = new DateTime(2001, 5, 15) },
            new() { Name = longName, BirthDate = new DateTime(1995, 3, 10) },
            new() { Name = "Bob", BirthDate = new DateTime(1998, 12, 25) }
        };
        var expected = persons[1]; // Person with long name

        // Act
        Person actual = _examples.GetTheOnlyOneWithANameLongerThan20Characters(persons);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [MonitoredTest]
    public void GetTheOnlyOneWithANameLongerThan20Characters_NoPersonWithLongName_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var persons = new List<Person>
        {
            new() { Name = "John", BirthDate = new DateTime(2001, 5, 15) },
            new() { Name = "Alice", BirthDate = new DateTime(1995, 3, 10) },
            new() { Name = "Bob", BirthDate = new DateTime(1998, 12, 25) }
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            _examples.GetTheOnlyOneWithANameLongerThan20Characters(persons));
    }

    [MonitoredTest]
    public void GetTheOnlyOneWithANameLongerThan20Characters_MultiplePersonsWithLongNames_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var persons = new List<Person>
        {
            new() { Name = "FirstVeryLongNameWithMoreThan20Characters", BirthDate = new DateTime(2001, 5, 15) },
            new() { Name = "Alice", BirthDate = new DateTime(1995, 3, 10) },
            new() { Name = "SecondVeryLongNameWithMoreThan20Characters", BirthDate = new DateTime(1998, 12, 25) }
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            _examples.GetTheOnlyOneWithANameLongerThan20Characters(persons));
    }

    [MonitoredTest]
    public void GetUpperLowerCaseDictionary_ShouldReturnDictionaryWithUpperKeysAndLowerValues()
    {
        // Arrange
        string randomWord = Guid.NewGuid().ToString();
        var words = new List<string> { "Hello", "WORLD", "test", randomWord };
        var expected = new Dictionary<string, string>
        {
            { "HELLO", "hello" },
            { "WORLD", "world" },
            { "TEST", "test" },
            { randomWord.ToUpper(), randomWord.ToLower() }
        };

        // Act
        IDictionary<string, string> actual = _examples.GetUpperLowerCaseDictionary(words);

        // Assert
        Assert.That(actual, Is.EquivalentTo(expected));
    }

    [MonitoredTest]
    public void GetUpperLowerCaseDictionary_DuplicateWordsWithDifferentCasing_ShouldThrowArgumentException()
    {
        // Arrange
        var words = new List<string> { "Hello", "HELLO", "world" };

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            _examples.GetUpperLowerCaseDictionary(words));
    }
}