using System.Reflection;
using QuizApplication.Domain;

namespace QuizApplication.Tests;

[ExerciseTestClass("dotnet2", "5-MVC-EF", "QuizApplication", @"QuizApplication.Domain\Answer.cs;QuizApplication.Domain\Category.cs;QuizApplication.Domain\Question.cs")]
public class BasicDomainTests
{
    [MonitoredFact]
    public void _01_QuestionShouldContainCorrectProperties()
    {
        Type questionType = typeof(Question);

        PropertyInfo[] properties = questionType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
        Assert.Equal(4, properties.Length);
        Assert.Contains(properties, p => p.Name == "Id");
        Assert.Contains(properties, p => p.Name == "QuestionString");
        Assert.Contains(properties, p => p.Name == "CategoryId");
        Assert.Contains(properties, p => p.Name == "Answers");

        ConstructorInfo[] constructors = questionType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
        Assert.Single(constructors);
        ParameterInfo[] parameters = constructors[0].GetParameters();
        Assert.Empty(parameters);
    }

    [MonitoredFact]
    public void _02_AnswerShouldContainCorrectProperties()
    {
        Type answerType = typeof(Answer);

        PropertyInfo[] properties = answerType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
        Assert.Equal(4, properties.Length);
        Assert.Contains(properties, p => p.Name == "Id");
        Assert.Contains(properties, p => p.Name == "AnswerText");
        Assert.Contains(properties, p => p.Name == "IsCorrect");
        Assert.Contains(properties, p => p.Name == "QuestionId");

        ConstructorInfo[] constructors = answerType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
        Assert.Single(constructors);
        ParameterInfo[] parameters = constructors[0].GetParameters();
        Assert.Empty(parameters);
    }

    [MonitoredFact]
    public void _03_NullChecks()
    {
        Question question = new Question();
        Assert.NotNull(question.Answers);
        Assert.NotNull(question.QuestionString);

        Answer answer = new Answer();
        Assert.NotNull(answer.AnswerText);
    }
}
