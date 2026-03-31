namespace Bean;

public class RequiresAddon : Attribute
{
    public Type AddonType;
    
    public RequiresAddon(Type addonType)
    {
        this.AddonType = addonType;
    }
}