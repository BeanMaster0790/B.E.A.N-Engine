using Microsoft.Xna.Framework;

namespace Bean.JsonVariables;

public struct JsonColour
{
    public int R = 0;
    public int G = 0;
    public int B = 0;
    public int A = 0;

    public JsonColour()
    {
    }

    public Color ToColor()
    {
        return new Color(R, G, B, A);
    }

    public static JsonColour FromColor(Color color)
    {
        return new JsonColour() {R =  color.R, G = color.G, B = color.B, A = color.A};
    }
}