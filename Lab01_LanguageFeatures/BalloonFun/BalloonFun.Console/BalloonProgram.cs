namespace BalloonFun.Console;

public class BalloonProgram
{
    private IOutputWriter _output;
    private Random _random;

    public BalloonProgram(IOutputWriter output)
    {
    }

    public void Run()
    {
        var randomBalloons = new Balloon[5];
        for (int i = 0; i < randomBalloons.Length; i++)
        {
            randomBalloons[i] = null; //TODO: assign a random balloon with a maximum size of 100
        }
        WriteBalloons(randomBalloons);

        Balloon poppedBalloon = null; //TODO: pick a random balloon from the array

        poppedBalloon.Baptize("Christian Baleoon");

        string nameToWrite = null; //TODO: get the name of the balloon or use "Anonymous" if it has no name
        _output.Write($"Popped balloon '{nameToWrite}' of size '{poppedBalloon.Size}' and color '{poppedBalloon.Color}'");
    }

    private void WriteBalloons(Balloon[] balloons)
    {
        foreach (Balloon balloon in balloons)
        {
            _output.Write($"A balloon of size '{balloon.Size}' and color '{balloon.Color}'");
        }
    }
}