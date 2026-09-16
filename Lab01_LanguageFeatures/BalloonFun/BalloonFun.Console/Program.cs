namespace BalloonFun.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
            IOutputWriter output = new ConsoleWriter();
            var balloonProgram = new BalloonProgram(output);
            balloonProgram.Run();

            System.Console.WriteLine();
            System.Console.WriteLine("Press enter to close...");
            System.Console.Read();
        }
    }
}