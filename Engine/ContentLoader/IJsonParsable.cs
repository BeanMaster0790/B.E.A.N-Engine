namespace Bean;


public interface IJsonParsable
{
    public string ExportJson();
    
    public void UpdateFromJson(string json);
}

public interface IJsonParsable<T> : IJsonParsable where T : IJsonParsable<T>
{
    static abstract T Parse(string json);
}