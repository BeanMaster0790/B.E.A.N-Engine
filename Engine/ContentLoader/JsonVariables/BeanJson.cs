namespace Bean.JsonVariables;

public interface IBeanJson
{
    public string Name { get; set; }
}

public struct BasicBeanJson : IBeanJson
{
    public string Name { get; set; }
}