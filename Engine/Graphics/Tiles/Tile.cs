using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;

namespace Bean.Graphics.Tiles
{
    public class Tile
    {
        public int textureMapValue;
        
        public Vector2 MapPosition;
        
        public Vector2 DrawPosition;

        public Rectangle TextureRectangle;

        public float Layer;
    }
}
