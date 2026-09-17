using Guts.Client.NUnit;
using LinqExamples.Models;

namespace LinqExamples.Tests;

[ExerciseTestFixture("dotNet2", "2-Linq", "LinqExamples", @"LinqExamples\WhereExamples.cs")]
public class WhereExamplesTests
{
    private WhereExamples _examples = null!;

    [SetUp]
    public void Setup()
    {
        _examples = new WhereExamples();
    }

    [MonitoredTest] public void FilterOutNumbersDivisibleByTen_ShouldOnlyReturnNumbersDivisibleByTen()
    {
        //Arrange
        int[] numbers = { 10, 11, 15, 20, 100, 101 };
        int[] expected = { 10, 20, 100 };

        //Act
        var actual = _examples.FilterOutNumbersDivisibleByTen(numbers);

        //Assert
        Assert.That(actual, Is.EquivalentTo(expected));
    }

    [MonitoredTest]
    public void PersonsThatAreEighteenOrOlder_CanBeFilteredOutUsingWhere()
    {
        //Arrange
        var persons = new List<Person>
        {
            new() {Name = "John", BirthDate = DateTime.Now.AddYears(-11)},
            new() {Name = "Jane", BirthDate = DateTime.Now.AddYears(-54)},
            new() {Name = "Jules", BirthDate = DateTime.Now.AddYears(-17)},
            new() {Name = "Jeffry", BirthDate = DateTime.Now.AddYears(-20)},
            new() {Name = "Joe", BirthDate = DateTime.Now.AddYears(-15)}
        };

        //Act
        var adults = _examples.FilterOutPersonsThatAreEighteenOrOlder(persons);

        //Assert
        Assert.That(adults, Has.All.Matches((Person p) => p.Name == "Jane" || p.Name == "Jeffry"));
    }

    [MonitoredTest]
    public void PersonsBornInFebruaryInAYearDividableBy4_CanBeFilteredOutUsingWhere()
    {
        //Arrange
        var persons = new List<Person>
        {
            new() {Name = "John", BirthDate = new DateTime(2000, 2, 15)},
            new() {Name = "Jane", BirthDate = new DateTime(2000, 3, 15)},
            new() {Name = "Jules", BirthDate = new DateTime(1996, 2, 8)},
            new() {Name = "Jeffry", BirthDate = new DateTime(1997, 2, 28)},
            new() {Name = "Joe", BirthDate = new DateTime(1998, 11, 1)}
        };

        //Act
        var nonLeapYearPersons = _examples.FilterOutPersonsBornInFebruaryInAYearDividableBy4(persons);

        //Assert
        Assert.That(nonLeapYearPersons, Has.All.Matches((Person p) => p.Name == "Jane" || p.Name == "Jeffry" || p.Name == "Joe"));
    }
}