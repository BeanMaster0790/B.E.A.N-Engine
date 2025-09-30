using System.Collections.Generic;
using Bean;
using Bean.Graphics;
using Bean.Noise;
using Bean.Scenes;
using DemoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DemoGame
{
    public class Game : Engine
    {
        public GameSettings Settings;

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            SceneManager.Instance.ActiveScene.Draw(spriteBatch);
        }

        public override void Load()
        {
            this.Settings = new GameSettings();

            GameWorld world = new GameWorld("DemoWorld");

            SceneManager.Instance.AddNewScene(world);

            MainMenu mainMenu = new MainMenu("MainMenu");

            SceneManager.Instance.AddNewScene(mainMenu);

            SceneManager.Instance.LoadScene("MainMenu");

            SceneManager.Instance.SetActiveScene("MainMenu");            
        }

        public override void Update()
        {
            SceneManager.Instance.ActiveScene.Update();

            if (this.Settings.ResolutionScale == 0)
            {
                switch (GraphicsManager.Instance.GetGameResolution().Y)
                {
                    case >= 2160:
                        SceneManager.Instance.ActiveScene.UIScene.RenderScale = 1.5f;
                        break;
                    case >= 1440:
                        SceneManager.Instance.ActiveScene.UIScene.RenderScale = 1.25f;
                        break;
                    case >= 1080:
                        SceneManager.Instance.ActiveScene.UIScene.RenderScale = 1f;
                        break;
                    case >= 720:
                        SceneManager.Instance.ActiveScene.UIScene.RenderScale = 0.75f;
                        break;
                    default:
                        SceneManager.Instance.ActiveScene.UIScene.RenderScale = 0.5f;
                        break;
                }
            }
            else
            {
                SceneManager.Instance.ActiveScene.UIScene.RenderScale = this.Settings.ResolutionScale;
            }
        }

        public override void LateUpdate()
        {
            base.LateUpdate();

            SceneManager.Instance.ActiveScene.LateUpdate();
        }
    }
}
