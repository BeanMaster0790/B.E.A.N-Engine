using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bean.Graphics
{
    public static class Shapes
    {
        private static Dictionary<string, Texture2D> _renderedTextures = new Dictionary<string, Texture2D>();

        public static int VectorToIndex(Vector2 point, Texture2D texture)
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

        public static Texture2D Circle(Circle circle)
        {
            string key = $"Circle:{circle.Circumference},{circle.Radius},{circle.Colour}";

            if (_renderedTextures.ContainsKey(key))
            {
                return _renderedTextures[key];
            }

            Texture2D texture = new Texture2D(GraphicsManager.Instance.GraphicsDevice, circle.Radius * 2, circle.Radius * 2);

            Color[] colours = new Color[texture.Width * texture.Height];

            for (int y = -circle.Radius; y <= circle.Radius; y++)
            {
                for (int x = -circle.Radius; x <= circle.Radius; x++)
                {
                    float dist = MathF.Sqrt(x * x + y * y);

                    if (circle.Filled)
                    {
                        if (dist <= circle.Radius)
                        {
                            Color colour = circle.Colour;

                            colour.A = 25;

                            colours[VectorToIndex(new Vector2(x, y), texture)] = colour;
                        }
                    }
                    else
                    {
                        if (dist >= circle.Radius - 3 && dist <= circle.Radius)
                        {
                            Color colour = circle.Colour;

                            colour.A = 25;

                            colours[VectorToIndex(new Vector2(x, y), texture)] = colour;
                        }
                    }
                }
            }

            texture.SetData(colours);

            _renderedTextures.Add(key, texture);

            return texture;
        }
        
        public static Texture2D Circle(Circle circle, bool filled = false)
		{
			circle.Filled = filled;

            return Circle(circle);	
		}

		public static Texture2D Circle(Circle circle, Color color, bool filled = false)
		{
			circle.Colour = color;

			return Circle(circle, filled);
		}

		public static Texture2D Circle(int radius, Color colour, bool filled = false)
		{
			Circle circle = new Circle(Vector2.Zero, radius, colour);

			return Circle(circle, filled);
		}
    }

    public class Circle
    {
        public Vector2 Position;

        public int Radius;

        public int Circumference;

        public Color Colour = Color.White;

        public bool Filled;

        public Circle(Vector2 position, int radius)
        {
            this.Position = position;

            this.Radius = radius;

            this.Circumference = this.Radius * 2;
        }

        public Circle(Vector2 position, int radius, Color colour) : this(position, radius)
        {
            this.Colour = colour;
        }
    }

    public class Line
    {
        public Vector2 StartPoint;
		public Vector2 EndPoint;

        public Vector2 Direction;

        public float Angle;

        public int Length;

        public Color Colour;

        public int Thickness;

        public Line(Vector2 startPoint, Vector2 endPoint)
		{
			this.StartPoint = startPoint;
			this.EndPoint = endPoint;

            this.Direction = this.EndPoint - this.StartPoint;

            this.Direction.Normalize();

            this.Angle = MathF.Atan2(this.Direction.Y, this.Direction.X);

            this.Angle = MathHelper.ToDegrees(this.Angle);

            this.Length = (int)Vector2.Distance(this.StartPoint, this.EndPoint);

            this.Colour = Color.White;
		}

        public Line(Vector2 startPoint, Vector2 endPoint, Color colour, int thickness) : this(startPoint, endPoint)
        {
            this.Colour = colour;
            this.Thickness = thickness;
        }

        public Line(Vector2 startPoint, float angle, int length)
        {
            this.StartPoint = startPoint;

            this.Angle = angle;

            angle = MathHelper.ToRadians(angle);

            this.Direction = new Vector2(MathF.Cos(angle), MathF.Sin(angle));

            this.Direction.Normalize();

            this.Length = length;

            this.EndPoint = startPoint + this.Direction * this.Length;
		}

		public Line(Vector2 startPoint, float angle,int length , Color colour, int thickness) : this(startPoint, angle, length)
		{
			this.Colour = colour;
			this.Thickness = thickness;
		}

	}

}
