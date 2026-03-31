using Bean.Graphics;
using Bean.PhysicsSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;

namespace Bean.Graphics.Lighting
{
	public class LightingManager
	{
		private Camera _camera;

		private List<Light> _sceneLights = new List<Light>();

		private RenderTarget2D _lightMap;

		public Color GlobalColour = new Color(255,255,255);

		private Dictionary<string, Texture2D> _renderedTextures = new Dictionary<string, Texture2D>();

		public LightingManager(Camera camera)
		{
			this._camera = camera;

			SetMapSize(this, null);

			this._camera.SizeChange += SetMapSize;
		}

		public void SetMapSize(object sender, EventArgs args)
		{
			this._lightMap?.Dispose();

			Vector2 screenSize = new Vector2(this._camera.GetWidth(), this._camera.GetHeight());

			this._lightMap = new RenderTarget2D(GraphicsManager.Instance.GraphicsDevice, (int)MathF.Ceiling(screenSize.X), (int)MathF.Ceiling(screenSize.Y));
		}

		public void AddLight(Light light)
		{
			this._sceneLights.Add(light);
		}

		public void RemoveLight(Light light) 
		{
			this._sceneLights.Remove(light);
		}		

		public RenderTarget2D DrawLightingMap(SpriteBatch spriteBatch, BasicEffect effect, RenderTarget2D cameraTarget)
		{
			GraphicsManager.Instance.GraphicsDevice.SetRenderTarget(this._lightMap);

			GraphicsManager.Instance.GraphicsDevice.Clear(this.GlobalColour);

			spriteBatch.Begin(samplerState: SamplerState.PointClamp, rasterizerState: RasterizerState.CullNone, effect: effect, sortMode: SpriteSortMode.FrontToBack);

			foreach (Light light in this._sceneLights)
			{
				if(light.IsVisable)
					DrawLight(light, spriteBatch);
			}

			spriteBatch.End();

			GraphicsManager.Instance.GraphicsDevice.SetRenderTarget(cameraTarget);

			return this._lightMap;
		}
		
		public virtual int VectorToIndex(Vector2 point, Texture2D texture)
		{
			Vector2 center = new Vector2(texture.Width / 2, texture.Height / 2);

			int adjustedX = (int)((int)point.X + center.X);
			int adjustedY = (int)((int)point.Y + center.Y);

			if (adjustedX < 0) adjustedX += texture.Width;
			else if (adjustedX >= texture.Width) adjustedX -= texture.Width;

			if (adjustedY < 0) adjustedY += texture.Height;
			else if (adjustedY >= texture.Height) adjustedY -= texture.Height;

			return adjustedY * texture.Width + adjustedX;
		}


		public void DrawLight(Light light, SpriteBatch spriteBatch)
        {
            string lightKey = $"{light.Distance}{light.Intencity}{light.Colour.PackedValue}";

			if (!this._renderedTextures.ContainsKey(lightKey))
			{
				GenerateLight(light, lightKey);
			}

			Texture2D texture = this._renderedTextures[lightKey];

			spriteBatch.Draw(texture, light.Parent.PropTransform.GetDrawPosition(), null, Color.White, 0f, new Vector2(texture.Width / 2, texture.Height / 2), 1, SpriteEffects.None, 0);
        }

        private void GenerateLight(Light light, string lightKey)
        {
            int radius = light.Distance;

            Texture2D texture = new Texture2D(GraphicsManager.Instance.GraphicsDevice, radius * 2, radius * 2);

            Color[] colours = new Color[texture.Width * texture.Height];

            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    float dist = MathF.Sqrt(x * x + y * y);

                    if (dist <= radius)
                    {
                        float stepCount = 4f; // number of brightness bands
                        float stepped = MathF.Floor((1f - (dist / radius)) * stepCount) / stepCount;
                        float alpha = stepped;

                        Color colour = light.Colour * alpha * light.Intencity;

                        colours[VectorToIndex(new Vector2(x, y), texture)] = colour;
                    }
                }
            }

            texture.SetData(colours);

            this._renderedTextures.Add(lightKey, texture);
        }


    }
}
