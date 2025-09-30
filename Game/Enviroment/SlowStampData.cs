using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DemoGame
{
    public class SlowStampData
    {
        public Rectangle TextureRectangle;

        public Vector2 Position;

        public SlowStampData(Vector2 position, Rectangle rectangle)
        {
            this.Position = position;

            this.TextureRectangle = rectangle;
        }
    }
}