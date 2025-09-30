using System;
using System.Collections.Generic;
using Bean;
using Bean.Debug;
using Bean.Graphics;
using Bean.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Random = Bean.Random;

namespace DemoGame
{
    public class MainMenu : Scene
    {

        public MenuHandler MenuHandler;

        public bool StartGame;

        public Dictionary<string, Menu> Menus = new Dictionary<string, Menu>();

        public MainMenu(string name) : base(name)
        {

        }

        public override void LateUpdate()
        {
            base.LateUpdate();

            this.MenuHandler.LateUpdate();
        }

        public override void LoadScene(object caller = null)
        {
            base.LoadScene(caller);

            this.MenuHandler = new MenuHandler(this, this.UIScene);

            this.MenuHandler.LoadScene();

            Game game = (Game)Engine.Instance;

            this.UIScene.RenderScale = game.Settings.ResolutionScale;

            foreach (var i in Engine.Instance.GraphicsDevice.Adapter.SupportedDisplayModes)
            {
                DebugServer.Log($"{i.Width},{i.Height} : ({i.AspectRatio})", this);
            }

            DemoWorldTestMap map = new DemoWorldTestMap(true)
            {
                TileScale = 2f
            };

            this.LightingManager.GlobalColour = new Color(255, 100, 60);

            this.Camera.EnableLighting = true;

            this.AddToScene(map);

            Texture2D pixel = new Texture2D(GraphicsManager.Instance.GraphicsDevice, 1, 1);

            for (int i = 0; i < 100; i++)
            {
                FireFly sprite = new FireFly()
                {
                    Texture = pixel,
                    Layer = 0.03f,

                    Position = new Vector2(Random.RandomInt(-519, 519), Random.RandomInt(-519, 519))
                };

                sprite.AddAddon(new FlickeringLight(0.25f, 10, 5, 3, 0.5f, Color.Orange));

                this.AddToScene(sprite);
            }

            this.Camera.SetZ(this.Camera.GetZFromHeight(150));

            this.Camera.Position = new Vector2(300, 300);
        }

        public override void Update()
        {
            base.Update();

            if (this.StartGame)
            {
                SceneManager.Instance.LoadScene("DemoWorld");
                SceneManager.Instance.SetActiveScene("DemoWorld");

                SceneManager.Instance.UnloadScene("MainMenu");    
            }

        }
    }
}