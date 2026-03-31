using System.Text;
using Bean.Debug;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace Bean.Graphics.Tiles
{
    public class Map : Addon
    {
        private RawMapData _rawMapData;

        private Texture2D[] _textures;

        private int _tileWidth;
        private int _tileHeight;

        private float _tileScale = 1;

        public float FrameSpeed;

        private float _frameTimer;
        private int _currentFrame;

        private Rectangle[] _rectangles;

        private List<Tile> _tiles = new List<Tile>();

        public Map(string name, string textureDirectory, string mapPath) :  base(name)
        {
            string data = File.OpenText(FileManager.TiledMapPath + mapPath).ReadToEnd();

            this._rawMapData = JsonConvert.DeserializeObject<RawMapData>(data);

			this._tileWidth = (int)(this._rawMapData.tilewidth * this._tileScale);
			this._tileHeight = (int)(this._rawMapData.tileheight * this._tileScale);

			this._rectangles = this.GetTileRectangles();
            
            GetTexturesFromDirectory(textureDirectory);

            LoadMap();
        }

        public Map(string name, string textureDirectory, RawMapData rawMapData) : base(name)
        {
            this._rawMapData = rawMapData;

            this._tileWidth = (int)(this._rawMapData.tilewidth * this._tileScale);
            this._tileHeight = (int)(this._rawMapData.tileheight * this._tileScale);

            GetTexturesFromDirectory(textureDirectory);
            
            this._rectangles = this.GetTileRectangles();

            LoadMap();
        }

        public Tile GetTile(float x, float y)
        {
            x *= this._tileWidth;
            y *= this._tileHeight;
            
            return this._tiles.FirstOrDefault(T => T.MapPosition.X == x && T.MapPosition.Y == y);
        }

		private void GetTexturesFromDirectory(string textureDirectory, bool isAnimated = false)
        {
            if (!isAnimated)
            {
                Texture2D texture = FileManager.LoadFromFile<Texture2D>(textureDirectory);

                this._textures = new Texture2D[1];

                this._textures[0] = texture;
            }
            else
            {
                int index = 1;

                List<Texture2D> textures = new List<Texture2D>();

                while (true)
                {
                    try
                    {
                        Texture2D texture = FileManager.LoadFromFile<Texture2D>(textureDirectory + index);

                        textures.Add(texture);

                        index++;

                        continue;
                    }
                    catch
                    {
                        break;
                    }
                }

                if (index != 1)
                    this._textures = textures.ToArray();
                else
                    throw new FileNotFoundException(textureDirectory + index);
            }
        }

        protected Rectangle[] GetTileRectangles()
        {
            int tilesOnWidth = this._textures[0].Width / (int)(this._tileWidth / this._tileScale);
            int tilesOnHeight = this._textures[0].Height / (int)(this._tileHeight / this._tileScale);

            int numberOfTiles = tilesOnWidth * tilesOnHeight;

            Rectangle[] rectangles = new Rectangle[numberOfTiles];

            int xIndex = -1;
            int yIndex = 0;

            for (int i = 0; i < numberOfTiles; i++)
            {

                xIndex++;

                if (xIndex >= tilesOnWidth)
                {
                    xIndex = 0;
                    yIndex++;
                }

                rectangles[i] = new Rectangle(xIndex * (int)(this._tileWidth / this._tileScale),
                    yIndex * (int)(this._tileHeight / this._tileScale),
                    (int)(this._tileWidth / this._tileScale), (int)(this._tileHeight / this._tileScale));

            }

            return rectangles;
        }

        private void LoadMap()
        {
            foreach (LayerData layer in this._rawMapData.layers)
            {
                int xIndex = 0;
                int yIndex = 0;

                if (layer.data != null)
                {
                    for (int i = 0; i < layer.data.Length; i++)
                    {

                        if (layer.data[i] == 0)
                        {
                            xIndex++;

                            if (xIndex >= this._rawMapData.width)
                            {
                                xIndex = 0;
                                yIndex++;
                            }

                            continue;
                        }

                        LoadTile(layer, xIndex, yIndex, i);

                        xIndex++;

                        if (xIndex >= this._rawMapData.width)
                        {
                            xIndex = 0;
                            yIndex++;
                        }
                    }
                }

                else if (layer.objects != null)
                {
                    foreach (TileObject obj in layer.objects)
                    {
                        LoadObject(obj, layer);
                    }
                }
            }
        }

        protected virtual void LoadObject(TileObject obj, LayerData layer)
        {
            //throw new NotImplementedException();
        }

        private void LoadTile(LayerData layer, int xIndex, int yIndex, int i)
        {
            if (!string.IsNullOrEmpty(layer.name))
            {
                Tile tile = new Tile();

                tile.TextureRectangle = this._rectangles[layer.data[i] - 1];
                
                tile.textureMapValue = layer.data[i];

                tile.MapPosition = new Vector2(xIndex * this._tileWidth, yIndex * this._tileHeight);

                tile.DrawPosition = tile.MapPosition -
                                    new Vector2(this._rawMapData.width / 2 * this._tileWidth,
                                        this._rawMapData.height / 2 * this._tileHeight) -
                                    new Vector2(this._tileWidth / 2, this._tileHeight / 2);
                
                ModifyTile(tile);

                this._tiles.Add(tile);
                
            }
        }

        protected virtual void ModifyTile(Tile tile)
        {
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (Tile tile in this._tiles)
            {
                spriteBatch.Draw(this._textures[this._currentFrame], tile.DrawPosition + this.Parent.PropTransform.Position -
                    this.Parent.Scene.Camera.Position, tile.TextureRectangle, Color.White, 0, Vector2.Zero, this._tileScale, SpriteEffects.None, tile.Layer);
            }
        }

        public override void Update()
        {
            base.Update();

            if (this.FrameSpeed == 0)
                return;

            this._frameTimer += Time.Instance.DeltaTime;

            if (this._frameTimer > this.FrameSpeed)
            {
                this._currentFrame += 1;
                this._frameTimer = 0;
            }


            if (this._currentFrame >= this._textures.Length)
                this._currentFrame = 0;
        }
    }
}
