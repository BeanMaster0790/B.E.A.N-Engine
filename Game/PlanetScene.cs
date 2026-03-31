using Bean;
using Bean.Debug;
using Bean.Graphics;
using Bean.Graphics.Animations;
using Bean.Graphics.Lighting;
using Bean.Graphics.Tiles;
using Bean.Noise;
using Bean.PhysicsSystem;
using Bean.Player;
using Bean.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Random = Bean.Random;

namespace DemoGame;

public class PlanetScene : Scene
{
    private struct Chunk
    {
        public WorldProp ChunkProp;

        public List<WorldProp> ChunkObjects;
    };
    
    public PerlinNoise HeightNoise;
    public PerlinNoise TempNoise;
    public PerlinNoise HumidityNoise;
    
    private int _chunkWidth = 16;
    private int _chunkHeight = 16;
    
    private int _minX = -8,  _maxX = 8;
    private int _minY = -8,  _maxY = 8;
    
    private Dictionary<Vector2, Chunk> _loadedChunks = new Dictionary<Vector2, Chunk>();
    
    public PlanetScene(string name) : base(name)
    {
        
    }

    public override void LoadScene(object caller = null)
    {
        base.LoadScene(caller);
        
        HeightNoise = new PerlinNoise(Random.RandomInt(-1000000, 10000000))
        {
            Amplitude = 1.0f,
            Frequency = 0.02f,   // large continents
            Octaves = 6,
            Persistence = 0.5f,
            Scale = 0.08f,
            StrechX = 1,
            StrechY = 1,
        };

        TempNoise = new PerlinNoise(Random.RandomInt(-1000000, 10000000))
        {
            Amplitude = 1.0f,
            Frequency = 0.5f, 
            Octaves = 3,
            Persistence = 0.4f,
            Scale = 0.05f,
            StrechX = 2,
            StrechY = 1,
        };
        
            
            Random.SetSeededRandom(123);

            GenerateChunksAround(new Vector2(0,0));
            
            this.Camera.SetZ(this.Camera.GetZFromHeight(200));
            
            WorldProp playerProp = FileManager.LoadWorldPropFromFile("Player");
            //playerProp.AddAddon(new PlayerController("Player Controller"));
            
            this.AddToScene(playerProp);
    }

    public Vector2 WorldToChunkPosition(Vector2 worldPosition)
    {
        return new Vector2(MathF.Round(worldPosition.X / _chunkWidth / 16), MathF.Round(worldPosition.Y / _chunkHeight / 16));
    }

    public void GenerateChunksAround(Vector2 position)
    {
        Vector2 chunkPosition = WorldToChunkPosition(position);
        
        List<Vector2> chunkPositions = new List<Vector2>();

        int renderDistance = 4;

        for (int y = -renderDistance / 2; y < renderDistance / 2; y++)
        {
            for (int x = -renderDistance / 2; x < renderDistance / 2; x++)
            {
                chunkPositions.Add(chunkPosition + new Vector2(x, y));
            }
        }

        foreach (KeyValuePair<Vector2, Chunk> loadedChunk in _loadedChunks)
        {
            if(!chunkPositions.Contains(loadedChunk.Key))
                UnloadChunk(loadedChunk.Key);
        }

        foreach (Vector2 chunk in chunkPositions)
        {
            GenerateChunk(chunk);
        }
        
    }

    override public void Update()
    {
        base.Update();
        
        GenerateChunksAround(this.Camera.Position);

        //this.Camera.Position = new Vector2(this.Camera.Position.X + 45 * Time.Instance.DeltaTime, this.Camera.Position.Y + 45 * Time.Instance.DeltaTime);
    }
    
    private void GenerateChunk(Vector2 position)
    {
        if(_loadedChunks.ContainsKey(position))
            return;
        
        if(position.X < _minX || position.X > _maxX)
            return;
        if(position.Y < _minY || position.Y > _maxY)
            return;
        
        WorldProp chunk = new WorldProp($"Chunk {position}");
        
        chunk.PropTransform.Position = new Vector2(position.X * _chunkWidth * 16, position.Y * _chunkHeight * 16);
        
        List<int> tileData = new List<int>();
            
        for (int y = -(_chunkWidth / 2); y < _chunkHeight / 2; y += 1)
        {
            for (int x = -(_chunkWidth / 2); x < _chunkWidth / 2; x += 1)
            {
                Random.SetSeededRandom(x + y * 16);
                
                float height = HeightNoise.FractalNoise(x + position.X * 16, y + position.Y * 16);
                height = MathF.Pow(height, 0.55f);
                
                float temp = TempNoise.FractalNoise(x + position.X * 16, y + position.Y * 16);
                temp = MathF.Pow(temp, 0.35f);
                
                
                if (height < 0.1f)
                {
                    tileData.Add(3); // ocean
                }
                else if (height < 0.095f)
                {
                    tileData.Add(4);
                }
                else
                {
                    if (temp < 0.95f)
                    {
                        tileData.Add(1); // plains / cold forest
                    }
                    else
                    {
                        tileData.Add(2); // forest
                    }
                }
            }
        }
            
        Dictionary<string, int[]> tileMap = new Dictionary<string, int[]>();
            
        tileMap.Add("Floor", tileData.ToArray());
            
        RawMapData mapData = new RawMapData(_chunkWidth, _chunkHeight, 16, 16, tileMap, new Dictionary<string, TileObject[]>());
            
        Map map = new Map("Map", "Map/TestTileMap", mapData);
        
        chunk.AddAddon(map);
        
        //chunk.AddAddon(new Collider("Collider") {Width = _chunkWidth * 16, Height = _chunkHeight * 16});
        
        this.AddToScene(chunk);
        
        
        Random.SetSeededRandom(123);

        List<WorldProp> chunkObjects = new List<WorldProp>();

        for (int y = 0; y < 16; y++)
        {
            for (int x = 0; x < 16; x++)
            {
                Tile tile = map.GetTile(x, y);
                
                Vector2 spawnPos = tile.DrawPosition + chunk.PropTransform.Position;

                bool treeClose = false;

                foreach (WorldProp chunkObject in chunkObjects)
                {
                    if (Vector2.Distance(spawnPos, chunkObject.PropTransform.Position) < 48)
                    {
                        treeClose = true;
                        break;
                    }
                }
                
                if(treeClose)
                    continue;

                if (tile.textureMapValue == 2)
                {
                    if (Random.RandomInt(0, 100) >= 25)
                        continue;

                    WorldProp tree = FileManager.LoadWorldPropFromFile("JungleTree");

                    tree.PropTransform.Position = spawnPos;

                    this.AddToScene(tree);

                    chunkObjects.Add(tree);
                }

                else if (tile.textureMapValue == 1)
                {
                    if(Random.RandomInt(0, 100) > 7)
                        continue;
                    
                    WorldProp tree = FileManager.LoadWorldPropFromFile((Random.RandomInt(0, 2) == 1) ? "MediumTree" : "BigTree");
                        
                    tree.PropTransform.Position = spawnPos;
                        
                    this.AddToScene(tree);
                        
                    chunkObjects.Add(tree);
                }
            }
            
        }
        
        this._loadedChunks.Add(position, new Chunk() {ChunkProp = chunk, ChunkObjects = chunkObjects});
    }

    private void UnloadChunk(Vector2 position)
    {
        this._loadedChunks[position].ChunkProp.Destroy();

        foreach (WorldProp chunkObject in this._loadedChunks[position].ChunkObjects)
        {
            chunkObject.Destroy();
        }
        
        this._loadedChunks.Remove(position);
    }
}