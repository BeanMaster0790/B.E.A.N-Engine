using System;
using System.Collections.Generic;
using System.IO;
using Bean.Graphics.Tiles;
using Bean.PhysicsSystem;
using Bean.Player;
using Bean.Scenes;
using Bean.UI;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Bean.Graphics
{
    public class TilemapExporterScene : Scene
    {
        private string _tileMapPath;

        public static Texture2D Texture;

        private Rectangle[] _textureRectangles;
        private TileData[] _tileData;

        private int _tileWidth = 16;

        private int _tileHeight = 16;

        public TilemapExporterScene(string name) : base(name)
        {
        }

        public override void LoadScene(object caller = null)
        {
            base.LoadScene(caller);

            this.Name = "TileMapExporter";

            this.Camera.IsFreeCam = true;

            this.Camera.SetZ(this.Camera.GetZFromHeight(300));

            GraphicsManager.Instance.GraphicsChanged += (object sender, EventArgs e) =>
            {
                this.UIScene.RefernceWidth = (int)GraphicsManager.Instance.GetCurentWindowSize().X;
                this.UIScene.RefrenceHeight = (int)GraphicsManager.Instance.GetCurentWindowSize().Y;
            };

            Engine.Instance.Window.FileDrop += (object sender, FileDropEventArgs e) =>
            {
                this._tileMapPath = e.Files[0];

                this.ClearData();

                if (Texture != null)
                {
                    Texture.Dispose();
                    Texture = null;
                }

                Texture = Texture2D.FromFile(GraphicsManager.Instance.GraphicsDevice, this._tileMapPath);

                this.StartTileEditing();
            };

            UIAlignContainer container = new UIAlignContainer()
            {
                Width = 1280,
                Height = 50,
                AlignDirection = AlignDirection.Horizontal,
                Name = "OptionsContainer",
                Spacing = 5,
                isScrollable = true
            };

            this.UIScene.AddUIProp(container);

            UIAlignContainer TileWidth = CreateMapOption("TileWidth: ", "TileWidth", "16");
            TileWidth.Parent = container;

            UIAlignContainer TileHeight = CreateMapOption("TileHeight: ", "TileHeight", "16");
            TileHeight.Parent = container;

            UIAlignContainer saveButton = new UIAlignContainer()
            {
                Width = 100,
                Height = 50,
                Colour = Color.Green,
                Parent = container,
                VerticalAlign = VerticalAlign.Center
            };

            this.UIScene.AddUIProp(saveButton);

            UIText genText = new UIText()
            {
                Parent = saveButton,
                Text = "Save",
                Colour = Color.White
            };

            this.UIScene.AddUIProp(genText);

            saveButton.OnLeftClick += (object sender, EventArgs e) =>
            {
                SaveData();
            };

            UIText HelpText = new UIText()
            {
                LocalPosition = new Vector2(0, 60),
                Text = $"Drag image into window to start. Save: \"CTL + S\" Restart: \"CTL + R\"",
                Colour = Color.White
            }; 
            
            this.UIScene.AddUIProp(HelpText);

        }

        private void SaveData()
        {
            if (this._tileData == null)
                return;

            string json = JsonConvert.SerializeObject(new TilemapRules() { TileData = this._tileData, TileHeight = this._tileHeight, TileWidth = this._tileWidth });

            File.WriteAllText(this._tileMapPath + ".json", json);
        }

        private void ClearData()
        {
            this._textureRectangles = null;
            this._tileData = null;

            foreach (Prop prop in this._sceneProps)
            {
                prop.Destroy();
            }
        }

        public override void Update()
        {
            base.Update();

            if (InputManager.Instance.IsKeyHeld(Keys.LeftControl))
            {
                if (InputManager.Instance.WasKeyPressed(Keys.R))
                {
                    this.ClearData();

                    int inputtedWidth = int.Parse(this.UIScene.GetPropWithName<UIInputText>("TileWidth").InputtedText);
                    int inputtedHeight = int.Parse(this.UIScene.GetPropWithName<UIInputText>("TileHeight").InputtedText);

                    this._tileWidth = inputtedWidth;
                    this._tileHeight = inputtedHeight;

                    StartTileEditing();
                }
                else if (InputManager.Instance.WasKeyPressed(Keys.S))
                {
                    SaveData();
                }
            }
        }

        public void StartTileEditing()
        {
            if (Texture == null)
                return;

            this._textureRectangles = GetTileRectangles();

            this._tileData = new TileData[this._textureRectangles.Length];

            if (File.Exists(this._tileMapPath + ".json"))
            {
                TilemapRules rules = JsonConvert.DeserializeObject<TilemapRules>(File.ReadAllText(this._tileMapPath + ".json"));

                this._tileWidth = rules.TileWidth;
                this._tileHeight = rules.TileHeight;

                this.UIScene.GetPropWithName<UIInputText>("TileWidth").InputtedText = this._tileWidth.ToString();
                this.UIScene.GetPropWithName<UIInputText>("TileHeight").InputtedText = this._tileHeight.ToString();

                this._tileData = rules.TileData;

                for (int i = 0; i < _tileData.Length; i++)
                {
                    TileData tileData = this._tileData[i];
                    TileEditorSprite sprite = new TileEditorSprite()
                    {
                        TileData = tileData,
                        Index = i
                    };

                    this.AddToScene(sprite);
                }

                return;
            }

            for (int i = 0; i < _textureRectangles.Length; i++)
            {
                Rectangle rectangle = this._textureRectangles[i];

                TileData tileData = new TileData()
                {
                    TileRectangle = rectangle,
                    TileRules = new int[8]
                };

                this._tileData[i] = tileData;

                TileEditorSprite sprite = new TileEditorSprite()
                {
                    TileData = tileData,
                    Index = i
                };

                this.AddToScene(sprite);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        private UIAlignContainer CreateMapOption(string labelText, string name, string defaultValue)
        {
            var container = new UIAlignContainer
            {
                AlignDirection = AlignDirection.Horizontal,
                VerticalAlign = VerticalAlign.Center,
                HorizontalAlign = HorizontalAlign.Left,
                Width = 100,
                Height = 50,
                Colour = Color.Gray,
                Name = name + "Container"

            };

            var label = new UIText
            {
                Text = labelText,
                Parent = container,
                Colour = Color.White
            };

            var input = new UIInputText
            {
                Name = name,
                Colour = Color.White,
                InputtedText = defaultValue,
                Width = 100,
                Height = 50,
                Parent = container,
            };

            this.UIScene.AddUIProp(container);
            this.UIScene.AddUIProp(label);
            this.UIScene.AddUIProp(input);

            return container;
        }

        protected Rectangle[] GetTileRectangles()
        {
            int tilesOnWidth = Texture.Width / this._tileWidth;
            int tilesOnHeight = Texture.Height / this._tileHeight;

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

                rectangles[i] = new Rectangle(xIndex * (int)(this._tileWidth),
                    yIndex * (int)(this._tileHeight),
                    (int)(this._tileWidth), (int)(this._tileHeight));

            }

            return rectangles;
        }
    }

    public class TileEditorSprite : Sprite
    {
        public TileData TileData;

        public int Index;

        private int _spacing = 64;

        public override void Start()
        {
            base.Start();

            this._spacing += this.TileData.TileRectangle.Width;

            for (int i = 0; i < TileData.TileRules.Length; i++)
            {
                CustomShape shape = new CustomShape(16, 16);

                shape.SetPoints(new List<Vector2>()
                {
                    new Vector2(-8, -8),
                    new Vector2(8, -8),
                    new Vector2(8,8),
                    new Vector2(-8, 8)
                });

                TileEditorRuleButton button = new TileEditorRuleButton()
                {
                    Tile = this,
                    Texture = shape.Texture,
                    Index = i
                };

                if (i == 0)
                {
                    button.Position = new Vector2(this.Index * this._spacing - this.TileData.TileRectangle.Width, -this.TileData.TileRectangle.Height);
                }
                else if (i == 1)
                {
                    button.Position = new Vector2(this.Index * this._spacing, -this.TileData.TileRectangle.Height);
                }
                else if (i == 2)
                {
                    button.Position = new Vector2(this.Index * this._spacing + this.TileData.TileRectangle.Width, -this.TileData.TileRectangle.Height);
                }
                else if (i == 3)
                {
                    button.Position = new Vector2(this.Index * this._spacing - this.TileData.TileRectangle.Width, 0);
                }
                else if (i == 4)
                {
                    button.Position = new Vector2(this.Index * this._spacing + this.TileData.TileRectangle.Width, 0);
                }
                else if (i == 5)
                {
                    button.Position = new Vector2(this.Index * this._spacing - this.TileData.TileRectangle.Width, this.TileData.TileRectangle.Height);
                }
                else if (i == 6)
                {
                    button.Position = new Vector2(this.Index * this._spacing, this.TileData.TileRectangle.Height);
                }
                else if (i == 7)
                {
                    button.Position = new Vector2(this.Index * this._spacing + this.TileData.TileRectangle.Width, this.TileData.TileRectangle.Height);
                }

                this.Scene.AddToScene(button);
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TilemapExporterScene.Texture, new Vector2(this.Index * this._spacing, 0) - this.Scene.Camera.Position, this.TileData.TileRectangle, Color.White, 0, new Vector2(this.TileData.TileRectangle.Width / 2, this.TileData.TileRectangle.Height / 2), 1, SpriteEffects.None, 0);
        }
    }

    public class TileEditorRuleButton : Sprite
    {
        public TileEditorSprite Tile;

        public int Index;

        private int _state;

        public override void Start()
        {
            base.Start();

            this.Colour = Color.Gray;
            this.Scale = 0.5f;

            this._state = this.Tile.TileData.TileRules[this.Index];

            if (this._state == 0)
                this.Colour = Color.Gray;
            else if (this._state == 1)
                this.Colour = Color.Green;
            else if (this._state == -1)
                this.Colour = Color.Red;
        }

        public override void Update()
        {
            base.Update();

            if(!InputManager.Instance.WasLeftButtonPressed())
                return;

            Vector2 mousePos = this.Scene.Camera.ScreenToWorld(InputManager.Instance.MousePosition()) + this.GetOrigin();

            if(mousePos.X > this.Position.X && mousePos.X < this.Position.X + this.Texture.Width && mousePos.Y > this.Position.Y && mousePos.Y < this.Position.Y + this.Texture.Height)
            {
                this._state++;

                if(this._state == 2)
                    this._state = -1;

                if(this._state == 0)
                    this.Colour = Color.Gray;
                else if(this._state == 1)
                    this.Colour = Color.Green;
                else if(this._state == -1)
                    this.Colour = Color.Red;

                this.Tile.TileData.TileRules[this.Index] = this._state;

                Console.WriteLine(JsonConvert.SerializeObject(this.Tile.TileData));

            }
        }
    }
}