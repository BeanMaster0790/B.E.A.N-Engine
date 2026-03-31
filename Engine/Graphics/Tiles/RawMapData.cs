using System.Text;
using Bean.Debug;

namespace Bean.Graphics.Tiles
{
    public class RawMapData
    {
        public LayerData[] layers;

        public int tileheight;
        public int tilewidth;

        public int height;
        public int width;

        public RawMapData(){}

        public RawMapData(int width, int height, int tileheight, int tilewidth, Dictionary<string, int[]> layersAndData, Dictionary<string, TileObject[]> layersAndObjects)
        {
            this.width = width;
            this.height = height;
            this.tileheight = tileheight;
            this.tilewidth = tilewidth;
            
            DeclareLayers(layersAndData.Keys.ToArray());

            foreach (KeyValuePair<string, int[]> layersData in layersAndData)
            {
                AddTileDataToLayer(layersData.Key, layersData.Value);
            }

            foreach (KeyValuePair<string, TileObject[]> layersObjects in layersAndObjects)
            {
                AddObjectToLayer(layersObjects.Key, layersObjects.Value);
            }
        }
        
        public void DeclareLayers(string[] names)
        {
            layers = new LayerData[names.Length];

            int i = 0;
            foreach (string name in names)
            {
                layers[i] = new LayerData() { name = name };

                i++;
            }
        }

        public void AddTileDataToLayer(string layer, int[] tileData)
        {
            if(tileData.Length != width * height)
                DebugServer.LogWarning("Not enough tiles for this layer!", this);

            this.layers.FirstOrDefault(layerData => layerData.name == layer).data = tileData;
        }

        public void AddObjectToLayer(string layer, TileObject[] objectData)
        {
            foreach (TileObject tileObject in objectData)
            {
                AddObjectToLayer(layer, tileObject);
            }
        }

        public void AddObjectToLayer(string layer, TileObject objectData)
        {
            this.layers.FirstOrDefault(layerData => layerData.name == layer).objects.Add(objectData);
        }
    }

    public class LayerData
    {
        public int[] data;

        public List<TileObject> objects;

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
