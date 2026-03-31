using Microsoft.Xna.Framework;

namespace Bean.Graphics.Tiles
{
    public class TileData
    {
        public Rectangle TileRectangle;
        public int[] TileRules;
    }

    public class TilemapRules
    {
        public int TileWidth;
        public int TileHeight;
        public TileData[] TileData;
    }
}