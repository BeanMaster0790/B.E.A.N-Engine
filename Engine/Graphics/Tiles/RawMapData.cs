using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bean.Graphics.Tiles
{
    public class RawMapData
    {
        public LayerData[] layers;

        public int tileheight;
        public int tilewidth;

        public int height;
        public int width;
    }

    public class LayerData
    {
        public int[] data;

        public TileObject[] objects;

        public int id;

        public string name;
    }

    public class TileObject
    {
        public int id;

        public string name;

        public float x;
        public float y;

        public float width;
        public float height;

    }
}
