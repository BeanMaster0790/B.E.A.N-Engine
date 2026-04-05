namespace Bean.JsonVariables;

public interface ICustomJsonParse
{
    public object FromJson(string json);
    public string ToJson(object obj);
}

public interface ICustomJsonParse<T> :  ICustomJsonParse
{
    object ICustomJsonParse.FromJson(string json) => FromJson(json);
    
    public T FromJson(string json) ;
}