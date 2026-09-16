using System.Drawing;

namespace BalloonFun.Console;

public class RandomExtensions
{
    public Balloon NextBalloon(Random random, int maxSize)
    {
        throw new NotImplementedException();
    }

    public Balloon NextBalloonFromArray( Random random, Balloon[] balloons)
    {
        throw new NotImplementedException();
    }

    private static Color GenerateRandomColor(Random random)
    {
        int red = random.Next(0, 256);
        int green = random.Next(0, 256);
        int blue = random.Next(0, 256);
        return Color.FromArgb(red, green, blue);
    }
}