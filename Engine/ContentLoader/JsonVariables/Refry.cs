namespace Bean.JsonVariables;

public class Refry : Attribute
{
    public string[] ArgKeys { get; set; }
    
    public Refry(string argKeys)
    {
        argKeys = argKeys.Replace(" ", "");
        this.ArgKeys = argKeys.Split(',');

    }
}