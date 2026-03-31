using Bean.JsonVariables;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Bean;

public class Transform : Addon, IJsonParsable<Transform>
{
    public Vector2 Position =  Vector2.Zero;
    
    public float Rotation = 0;

    public Vector2 Scale = Vector2.One;

    public float Layer;

    public Transform(string name) : base(name)
    {
    }

    public Vector2 GetDrawPosition()
    {
        return new Vector2(Position.X, Position.Y) - this.Parent.Scene.Camera.Position;
    }
    
    public struct TransformJson : IBeanJson
    {
        public string Name { get; set; }

        public Vector2 Position { get; set; }
        
        public float Rotation { get; set; }
        
        public Vector2 Scale { get; set; }
        
        public float Layer { get; set; }
    }

    public static Transform Parse(string json)
    {
        TransformJson? transformNull = JsonConvert.DeserializeObject<TransformJson>(json);
        
        if(transformNull == null)
            throw new ArgumentException("Invalid JSON");
        
        TransformJson transformJson = (TransformJson)transformNull;
        
        Transform transform = new Transform(transformJson.Name)
        {
            Position = transformJson.Position,
            Rotation = transformJson.Rotation,
            Scale = transformJson.Scale,
            Layer = transformJson.Layer
        };
        
        return transform;
    }

    public void UpdateFromJson(string json)
    {
        TransformJson? transformNull = JsonConvert.DeserializeObject<TransformJson>(json);
        
        if(transformNull == null)
            throw new ArgumentException("Invalid JSON");
        
        TransformJson transformJson = (TransformJson)transformNull;
        
        this.Position = transformJson.Position;
        this.Rotation = transformJson.Rotation;
        this.Scale = transformJson.Scale;
        this.Layer = transformJson.Layer;
    }

    public string ExportJson()
    {
        TransformJson json = new TransformJson()
        {
            Name = this.Name,
            Position = this.Position,
            Rotation = this.Rotation,
            Scale = this.Scale,
            Layer = this.Layer
        };
        
        return JsonConvert.SerializeObject(json);
    }
}