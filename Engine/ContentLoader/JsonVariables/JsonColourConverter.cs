using Microsoft.Xna.Framework;

namespace Bean.JsonVariables;

public struct JsonColourConverter : ICustomJsonParse<Color>
{
    public Color FromJson(string json)
    {
        string[] colours = json.Split(',');
        
        return new Color(int.Parse(colours[0]), int.Parse(colours[1]), int.Parse(colours[2]), int.Parse(colours[3]));
    }

    public string ToJson(Object obj)
    {
        Color color = (Color)obj;
        
        return $"{color.R},{color.G},{color.B},{color.A}";
    }
}