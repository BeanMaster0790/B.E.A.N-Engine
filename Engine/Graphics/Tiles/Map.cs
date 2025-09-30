using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bean.Debug;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace Bean.Graphics.Tiles
{
    public class Map : Prop
    {
        public string MapDirectory 
        { 
            set
            {
                this._mapDirectory = FileManager.TiledMapPath + value; 
			}

            get
            {
                return this._mapDirectory.Replace(FileManager.TiledMapPath, "");
            }
                
         }

        private string _mapDirectory; 

        public string TextureDirectory 
        { 
            set 
            { 
                GetTexturesFromDirectory(value);
            }

            get
            {
                return this._textureDirectory;
            }
        }

        private string _textureDirectory;

        protected RawMapData _rawMapData;

        protected Texture2D[] _textures { get; set; }

        protected int _tileWidth;
        protected int _tileHeight;

        private float _tileScale;

        private int _numberOfTiles;

        public float FrameSpeed;

        private float _frameTimer;
        private int _currentFrame;

        public float TileScale
        {
            get
            {
                return this._tileScale;
            }
            set
            {
                _tileScale = value;
            }
        }

        protected Rectangle[] _rectangles;

        protected List<Tile> _tiles = new List<Tile>();

        public Map()
        {

        }

		public override void Start()
		{
			base.Start();

            if (this._rawMapData == null)
            {
                string data = File.OpenText(this._mapDirectory).ReadToEnd();

                this._rawMapData = JsonConvert.DeserializeObject<RawMapData>(data);             
            }

			this._tileWidth = (int)(this._rawMapData.tilewidth * this._tileScale);
			this._tileHeight = (int)(this._rawMapData.tileheight * this._tileScale);

			this._rectangles = this.GetTileRectangles();

			this._numberOfTiles = this._rawMapData.width * this._rawMapData.height;

            LoadMap();
		}

		private void GetTexturesFromDirectory(string textureDirectory)
        {

            this._textureDirectory = textureDirectory;

            try
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

                this._textures = textures.ToArray();
            }
            catch
            {
                Texture2D texture = FileManager.LoadFromFile<Texture2D>(textureDirectory);

                this._textures = new Texture2D[1];

                this._textures[0] = texture;
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

        public virtual void LoadMap()
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

        public virtual void LoadObject(TileObject obj, LayerData layer)
        {
            throw new NotImplementedException();
        }

        public virtual void LoadTile(LayerData layer, int xIndex, int yIndex, int i)
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (Tile tile in this._tiles)
            {
                spriteBatch.Draw(this._textures[this._currentFrame], tile.Position -
                    new Vector2(this._rawMapData.width / 2 * this._tileWidth, this._rawMapData.height / 2 * this._tileHeight) -
                    new Vector2(this._tileWidth / 2, this._tileHeight / 2) -
                    base.Scene.Camera.Position, tile.TextureRectangle, Color.White, 0, Vector2.Zero, this._tileScale, SpriteEffects.None, tile.Layer);
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
