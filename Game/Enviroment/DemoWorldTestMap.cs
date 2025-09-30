using System;
using System.Diagnostics.Tracing;
using Bean;
using Bean.Graphics;
using Bean.Graphics.Tiles;
using Bean.PhysicsSystem;
using Microsoft.Xna.Framework;

namespace DemoGame 
{
    class DemoWorldTestMap : Map
    {
        private bool _isMenu;

        public DemoWorldTestMap(bool isMenu)
        {
            this._isMenu = isMenu;
        }

        public override void Start()
        {
            this.TextureDirectory = "Tiles/TestMapTiles/MapTiles";
            this.MapDirectory = "Map.json";

            this.TileScale = 1;

            this.FrameSpeed = 0.2f;

            base.Start();
        }

        public override void LoadTile(LayerData layer, int xIndex, int yIndex, int i)
        {
            if (!string.IsNullOrEmpty(layer.name))
            {
                Tile tile = new Tile();

                tile.TextureRectangle = base._rectangles[layer.data[i] - 1];

                tile.Position = new Vector2(xIndex * base._tileWidth, yIndex * base._tileHeight);

                if (layer.name == "Floor")
                    tile.Layer = 0.001f;
                else if (layer.name == "Flowers")
                    tile.Layer = 0.002f;
                else if (layer.name == "Walls")
                {
                    tile.Layer = 0.003f;

                    WorldProp prop = new WorldProp()
                    {
                        Position = new Vector2(xIndex * base._tileWidth, yIndex * base._tileHeight) * base.TileScale - new Vector2(this._rawMapData.width / 2 * this._tileWidth, this._rawMapData.height / 2 * this._tileHeight) - new Vector2(this._tileWidth / 2, this._tileHeight / 2),
                        Name = "Wall",
                        Tag = "NoSpawn"
                    };

                    prop.AddAddon(new Collider() { Width = base._tileWidth, Height = base._tileHeight });

                    this.Scene.AddToScene(prop);
                }
                else
                    tile.Layer = 0.003f;

                base._tiles.Add(tile);
            }
        }

        public override void LoadObject(TileObject obj, LayerData layer)
        {       
            if (layer.name == "Tree")
                {
                    Tree tree = new Tree()
                    {
                        Name = "Tree",

                        Tag = "NoSpawn",

                        Position = new Vector2(obj.x, obj.y) * base.TileScale - new Vector2(this._rawMapData.width / 2 * this._tileWidth, this._rawMapData.height / 2 * this._tileHeight) -
                        new Vector2(this._tileWidth / 2, this._tileHeight / 2),

                        Scale = this.TileScale,
                    };

                    this.Scene.AddToScene(tree);
                }

            if (this._isMenu)
                return;

            if (layer.name == "House1")
            {
                House house = new House
                {
                    Name = "TargetHouse",

                    Tag = "NoSpawn",

                    Position = new Vector2(obj.x, obj.y) * base.TileScale - new Vector2(this._rawMapData.width / 2 * this._tileWidth, this._rawMapData.height / 2 * this._tileHeight) -
                    new Vector2(this._tileWidth / 2, this._tileHeight / 2) + new Vector2(obj.width / 2, obj.height / 2),

                    Scale = this.TileScale,

                    TexturePath = "Houses/HouseOne"
                };

                this.Scene.AddToScene(house);
            }

            else if (layer.name == "House2")
            {
                House house = new House
                {
                    Name = "TargetHouse",

                    Tag = "NoSpawn",

                    Position = new Vector2(obj.x, obj.y) * base.TileScale - new Vector2(this._rawMapData.width / 2 * this._tileWidth, this._rawMapData.height / 2 * this._tileHeight) -
                    new Vector2(this._tileWidth / 2, this._tileHeight / 2) + new Vector2(obj.width / 2, obj.height / 2),

                    Scale = this.TileScale,

                    TexturePath = "Houses/HouseTwo"
                };

                this.Scene.AddToScene(house);
            }

            else if (layer.name == "House3")
            {
                House house = new House
                {
                    Name = "TargetHouse",

                    Tag = "NoSpawn",

                    Position = new Vector2(obj.x, obj.y) * base.TileScale - new Vector2(this._rawMapData.width / 2 * this._tileWidth, this._rawMapData.height / 2 * this._tileHeight) -
                    new Vector2(this._tileWidth / 2, this._tileHeight / 2) + new Vector2(obj.width / 2, obj.height / 2),

                    Scale = this.TileScale,

                    TexturePath = "Houses/HouseThree"
                };

                this.Scene.AddToScene(house);
            }
            else if (layer.name == "NoSpawn")
            {
                WorldProp collider = new WorldProp
                {
                    Name = "NoSpawn",

                    Tag = "NoSpawn",

                    Position = new Vector2(obj.x, obj.y) * base.TileScale - new Vector2(this._rawMapData.width / 2 * this._tileWidth, this._rawMapData.height / 2 * this._tileHeight),
                };

                collider.AddAddon(new Collider() { Width = (int)obj.width, Height = (int)obj.height, IsSolid = false });

                this.Scene.AddToScene(collider);
            }

            else if (layer.name == "VillageBounds")
            {
                if (obj.name == "TurretPoint" && this.Scene is GameWorld world)
                {
                    world.MushroomSpawner.TurretPoints.Add(new Vector2(obj.x, obj.y) * base.TileScale - new Vector2(this._rawMapData.width / 2 * this._tileWidth, this._rawMapData.height / 2 * this._tileHeight), false);
                }
                else if (obj.name == "PoisonPoint" && this.Scene is GameWorld world1)
                {
                    world1.MushroomSpawner.PoisonPoints.Add(new Vector2(obj.x, obj.y) * base.TileScale - new Vector2(this._rawMapData.width / 2 * this._tileWidth, this._rawMapData.height / 2 * this._tileHeight), false);
                }
                else if(this.Scene is GameWorld world2)
                {
                    Vector2 position = new Vector2(obj.x, obj.y) * base.TileScale - new Vector2(this._rawMapData.width / 2 * this._tileWidth, this._rawMapData.height / 2 * this._tileHeight);

                    Rectangle rectangle = new Rectangle((int)position.X, (int)position.Y, (int)obj.width, (int)obj.height);

                    world2.MushroomSpawner.SpawnBounds = rectangle; 
                }

            }
        }
    }
}