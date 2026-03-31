using Bean.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Bean.Graphics
{
	public class CustomShape
	{
		public Texture2D Texture
		{

			get
			{
				if (_texture == null)
				{
					CreateTexture();
				}

				return _texture;
			}

		}

		private int _width;
		private int _height;

		private int _centerX;
		private int _centerY;

		public Color Colour;

		protected Texture2D _texture;

		protected bool _fillShape;

		protected Vector2[] _points;

		protected Vector2[] outlineVectors;

		protected Color[] _texData;

		public CustomShape(int width = 0, int height = 0, bool fillShape = true, Color? colour = null)
		{
			_fillShape = fillShape;

			_width = width;
			_height = height;

			if (colour == null)
				Colour = Color.White;
			else
				Colour = (Color)colour;
		}

		public void SetPoints(List<Vector2> points)
		{
			_points = new Vector2[points.Count];

			for (int i = 0; i < points.Count; i++)
			{
				Vector2 point = points[i];

				_points[i] = new Vector2((int)point.X, (int)point.Y);
			}

			_texture = null;
		}

		private void CreateTexture()
		{
			InitializeTexture();
			SetTextureOutline();

			if (_fillShape)
				SetTextureFill();

			_texture.SetData(_texData);
		}

		protected virtual void InitializeTexture()
		{
			if (_height == 0 && _width == 0)
				AutoCalculateSize();
			else
				_texture = new Texture2D(GraphicsManager.Instance.GraphicsDevice, _width, _height);

			_centerX = _texture.Width / 2;
			_centerY = _texture.Height / 2;
		}

		private void AutoCalculateSize()
		{
			int maxX = 0;
			int maxY = 0;

			int minX = 0;
			int minY = 0;

			foreach (Vector2 point in _points)
			{
				if (point.X > maxX) maxX = (int)point.X;

				if (point.Y > maxY) maxY = (int)point.Y;

				if (point.X < minX) minX = (int)point.X;

				if (point.Y < minY) minY = (int)point.Y;
			}

			_texture = new Texture2D(GraphicsManager.Instance.GraphicsDevice, maxX - minX, maxY - minY);
		}

		protected virtual void SetTextureOutline()
		{
			_texData = new Color[_texture.Width * _texture.Height];

			List<Vector2> filledPixels = new List<Vector2>();

			for (int i = 0; i < _points.Length; i++)
			{
				Vector2 startPoint = _points[i];

				Vector2 endPoint = new Vector2();

				Vector2 currentPoint = startPoint;

				if (i == _points.Length - 1)
					endPoint = _points[0];
				else
					endPoint = _points[i + 1];

				Vector2 distanceBetweenPoints = endPoint - startPoint;

				distanceBetweenPoints = new Vector2(Math.Abs(distanceBetweenPoints.X), Math.Abs(distanceBetweenPoints.Y));

				Vector2 steps = new Vector2(startPoint.X < endPoint.X ? 1 : -1, startPoint.Y < endPoint.Y ? 1 : -1);

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

				outlineVectors = filledPixels.ToArray();
			}

			ApplyFilledPixels(filledPixels);

		}

		protected virtual void SetTextureFill()
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

			ApplyFilledPixels(filledPixels);
		}

		public virtual int VectorToIndex(Vector2 point)
		{
			// Adjust the point to be relative to the center
			int adjustedX = (int)point.X + _centerX;
			int adjustedY = (int)point.Y + _centerY;

			// Wrap coordinates manually without using double modulus
			if (adjustedX < 0) adjustedX += _texture.Width;
			else if (adjustedX >= _texture.Width) adjustedX -= _texture.Width;

			if (adjustedY < 0) adjustedY += _texture.Height;
			else if (adjustedY >= _texture.Height) adjustedY -= _texture.Height;

			// Calculate the index using the wrapped coordinates
			return adjustedY * _texture.Width + adjustedX;
		}

		protected virtual void ApplyFilledPixels(IEnumerable<Vector2> pixels)
		{
			foreach (Vector2 point in pixels)
			{
				int index = VectorToIndex(point);
				if (index >= 0 && index < _texData.Length)
					_texData[index] = Colour;
			}
		}

	}
}
