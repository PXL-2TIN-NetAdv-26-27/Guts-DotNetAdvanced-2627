namespace BalloonFun.Console;

public class ConsoleWriter : IOutputWriter
{
    public void Write(string message)
    {
        System.Console.WriteLine(message);
    }
}