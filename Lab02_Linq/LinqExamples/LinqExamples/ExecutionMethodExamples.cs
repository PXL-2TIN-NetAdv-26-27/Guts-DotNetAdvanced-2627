using LinqExamples.Models;

namespace LinqExamples;

public class ExecutionMethodExamples
{
    public Person? GetFirstPersonBornBefore2000(IEnumerable<Person> persons)
    {
        //TODO: use ONE line of code to return the first person born before year 2000 or null if there is no such person

        throw new NotImplementedException("Use LINQ to implement this method");
    }

    public Person GetTheOnlyOneWithANameLongerThan20Characters(IEnumerable<Person> persons)
    {
        //TODO: use ONE line of code to return the only person with a name longer than 20 characters or if there is no such person or more than one such person, throw an exception
        
        throw new NotImplementedException("Use LINQ to implement this method");
    }

    public IDictionary<string, string> GetUpperLowerCaseDictionary(IEnumerable<string> words)
    {
        // TODO: use ONE linq statement to create a dictionary where the keys are the upper case words and the values are the lower case words

        throw new NotImplementedException("Use LINQ to implement this method");
    }
}