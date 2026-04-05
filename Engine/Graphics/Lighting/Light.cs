using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using Bean.JsonVariables;
using Newtonsoft.Json;

namespace Bean.Graphics.Lighting
{
	public class Light : Addon
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

	        public JsonColourConverter ColourConverter { get; set; }
	        public int Distance  { get; set; }
	        public float Intensity  { get; set; }
        }
	}
}
