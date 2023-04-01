public class ColorVector4
{
    public int Red, Blue, Green, Yellow;
    
    public ColorVector4(int red, int blue, int green, int yellow)
    {
        Red = red;
        Blue = blue;
        Green = green;
        Yellow = yellow;
    }
    
    public ColorVector4()
    {
        Red = 0;
        Blue = 0;
        Green = 0;
        Yellow = 0;
    }
    
    public static ColorVector4 operator +(ColorVector4 a, ColorVector4 b)
    {
        return new ColorVector4(a.Red + b.Red, a.Blue + b.Blue, a.Green + b.Green, a.Yellow + b.Yellow);
    }
    
    public static ColorVector4 operator -(ColorVector4 a, ColorVector4 b)
    {
        return new ColorVector4(a.Red - b.Red, a.Blue - b.Blue, a.Green - b.Green, a.Yellow - b.Yellow);
    }

    public override string ToString()
    {
        return $"({Red}, {Blue}, {Green}, {Yellow})";
    }
}
