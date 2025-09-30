using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public Light(float intencity, int distance, Color colour)
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
	}
}
