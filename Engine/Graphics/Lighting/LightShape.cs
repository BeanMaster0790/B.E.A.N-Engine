using Bean.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bean.Graphics.Lighting
{
	class LightShape : CustomShape
    {

		private float _fillIntensity;
		private float _falloff;

		public LightShape(Color colour, float intensity, int falloff) : base(fillShape: true, colour: colour)
		{ 
			this._fillIntensity = intensity;
			this._falloff = falloff;
		}

		protected override void SetTextureOutline()
		{
			this._texData = new Color[_texture.Width * _texture.Height];

			List<Vector2> filledPixels = new List<Vector2>();

			for (int i = 0; i < this._points.Length; i++)
			{
				Vector2 startPoint = this._points[i];

				Vector2 endPoint = new Vector2();

				Vector2 currentPoint = startPoint;

				if (i == this._points.Length - 1)
					endPoint = this._points[0];
				else
					endPoint = this._points[i + 1];

				Vector2 distanceBetweenPoints = endPoint - startPoint;

				distanceBetweenPoints = new Vector2(Math.Abs(distanceBetweenPoints.X), Math.Abs(distanceBetweenPoints.Y));

				Vector2 steps = new Vector2((startPoint.X < endPoint.X) ? 1 : -1, (startPoint.Y < endPoint.Y) ? 1 : -1);

				if (startPoint.X < endPoint.X)
					steps.X = 1;
				else if (startPoint.X == endPoint.X)
					steps.X = 0;
				else
					steps.X = -1;

				if (startPoint.Y < endPoint.Y)
					steps.Y = 1;
				else if (startPoint.Y == endPoint.Y)
					steps.Y = 0;
				else
					steps.Y = -1;

				int error = (int)(Math.Abs(distanceBetweenPoints.X) - Math.Abs(distanceBetweenPoints.Y));

				while (currentPoint != endPoint)
				{
					filledPixels.Add(currentPoint);

					int error2 = error * 2;

					if (error2 > -distanceBetweenPoints.Y)
					{
						error -= (int)distanceBetweenPoints.Y;
						currentPoint.X += steps.X;
					}

					if (error2 < distanceBetweenPoints.X)
					{
						error += (int)distanceBetweenPoints.X;
						currentPoint.Y += steps.Y;
					}
				}

				this.outlineVectors = filledPixels.ToArray();
			}
		}

		protected override void SetTextureFill()
		{
			var pointsByY = outlineVectors.GroupBy(p => p.Y).OrderBy(group => group.Key);

			List<Vector2> filledPixels = new List<Vector2>();

			foreach (var group in pointsByY)
			{
				int y = (int)group.Key;
				List<int> xValues = group.Select(p => (int)p.X).OrderBy(x => x).ToList();

				if (xValues.Count <= 1)
					continue;

				for (int i = 0; i < xValues.Count - 1; i += 1)
				{
					int startX = xValues[i];
					int endX = xValues[i + 1];

					for (int x = startX; x < endX; x++)
					{
						filledPixels.Add(new Vector2(x, y));
					}
				}

			}

			foreach (Vector2 point in filledPixels)
			{
				int Index = VectorToIndex(point);

				if (Index > this._texData.Length - 1)
					continue;

				float distanceFromCenter = Vector2.DistanceSquared(Vector2.Zero, point);

				if (distanceFromCenter < this._falloff)
				{
					this._texData[VectorToIndex(point)] = this.Colour;
					continue;
				}

				float multiplier = (this.Texture.Width / 2 - this._falloff) / distanceFromCenter;

				float noise = (float)(Random.RandomFloat(0,1) - 0.5) * 0.0040f;
				float ditheredMultiplier = multiplier + noise;

				
				ditheredMultiplier = Math.Clamp(ditheredMultiplier, 0, 1);

				this._texData[Index] = this.Colour * ditheredMultiplier * this._fillIntensity;
			}
		}
	}
}
