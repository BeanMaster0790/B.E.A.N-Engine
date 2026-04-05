using Bean.JsonVariables;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Bean;

public class Transform : Addon
{
    [Tinned("Position")]
    public Vector2 Position =  Vector2.Zero;
    
    [Tinned("Rotation")]
    public float Rotation = 0;

    [Tinned("Scale")]
    public Vector2 Scale = Vector2.One;

    [Tinned("Layer")]
    public float Layer;

    public Transform(string name) : base(name)
    {
    }

    public Vector2 GetDrawPosition()
    {
        return new Vector2(Position.X, Position.Y) - this.Parent.Scene.Camera.Position;
    }
}