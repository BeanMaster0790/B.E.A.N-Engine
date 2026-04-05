namespace Bean.JsonVariables;

public class Tinned : Attribute
{
    public Tinned(string key, bool constructorValue = false, int constructorIndex = -1, Type customParseType = null)
    {
        this.Key = key;
        this.ConstructorValue = constructorValue;
        this.ConstructorIndex = constructorIndex;
        this.customParseType =  customParseType;
    }
    
    public string Key { get; }
    public bool ConstructorValue { get; }
    
    public Type customParseType { get; }
    
    public int ConstructorIndex { get; }
}