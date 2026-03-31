using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using Bean.JsonVariables;
using Newtonsoft.Json;

namespace Bean.Graphics.Lighting
{
	public class Light : Addon, IJsonParsable<Light>
	{
		[DebugServerVariable]
		public float Intencity;

		[DebugServerVariable]
		public int Distance;

		[DebugServerVariable]
		public Color Colour;

		public Texture2D LightTexture;

		public Light(string name, float intencity, int distance, Color colour) : base(name)
		{
			this.Intencity = intencity;
			this.Distance = distance;
			this.Colour = colour;
		}

		public override void Start()
		{
			base.Start();

			base.Parent.Scene.LightingManager.AddLight(this);
		}

        public override void RemoveFromGame()
        {
            base.RemoveFromGame();

			base.Parent.Scene.LightingManager.RemoveLight(this);
        }
        
        public struct LightJson : IBeanJson
        {
	        public string Name { get; set; }

	        public JsonColour Colour { get; set; }
	        public int Distance  { get; set; }
	        public float Intensity  { get; set; }
        }

        public static Light Parse(string json)
        {
	        LightJson? lightJsonNull = JsonConvert.DeserializeObject<LightJson>(json);

	        if (lightJsonNull == null)
		        throw new ArgumentException("Invalid Json");
	        
	        LightJson lightJson = (LightJson)lightJsonNull;
	        
	        return new Light(lightJson.Name, lightJson.Intensity, lightJson.Distance,  lightJson.Colour.ToColor());
        }

        public void UpdateFromJson(string json)
        {
	        LightJson? lightJsonNull = JsonConvert.DeserializeObject<LightJson>(json);

	        if (lightJsonNull == null)
		        throw new ArgumentException("Invalid Json");
	        
	        LightJson lightJson = (LightJson)lightJsonNull;
	        
	        this.Name = lightJson.Name;
	        this.Colour = lightJson.Colour.ToColor();
	        this.Distance = lightJson.Distance;
	        this.Intencity = lightJson.Intensity;
        }

        public string ExportJson()
        {
	        LightJson json = new LightJson()
	        {
		        Name = this.Name,
		        Intensity = this.Intencity,
		        Colour = JsonColour.FromColor(this.Colour),
		        Distance = this.Distance,
	        };
	        
	        return JsonConvert.SerializeObject(json);
        }
	}
}
