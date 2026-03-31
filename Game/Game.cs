using Bean;
using Bean.Debug;
using Bean.Graphics;
using Bean.Noise;
using Bean.Scenes;
using Microsoft.Xna.Framework.Graphics;
using Bean.DebugScenes;
using Bean.Graphics.Animations;
using Bean.Graphics.Lighting;
using Bean.Graphics.Tiles;
using Bean.PhysicsSystem;
using Bean.Player;
using Microsoft.Xna.Framework;
using Random = Bean.Random;

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

            PlanetScene scene = new PlanetScene("PlanetScene");
            
            //scene.Camera.EnableLighting = true;
            scene.Camera.SetZ(scene.Camera.GetZFromHeight(2000));
            
            scene.LightingManager.GlobalColour = new Color(0.3f, 0.3f, 0.3f);
            
            SceneManager.Instance.AddNewScene(scene);
            
            SceneManager.Instance.LoadScene("PlanetScene");
            
            SceneManager.Instance.SetActiveScene("PlanetScene");
        }

        public override void Update()
        { 
            SceneManager.Instance.ActiveScene.Update();
            // if (this.Settings.ResolutionScale == 0)
            // {
            //     switch (GraphicsManager.Instance.GetGameResolution().Y)
            //     {
            //         case >= 2160:
            //             SceneManager.Instance.ActiveScene.UIScene.RenderScale = 1.5f;
            //             break;
            //         case >= 1440:
            //             SceneManager.Instance.ActiveScene.UIScene.RenderScale = 1.25f;
            //             break;
            //         case >= 1080:
            //             SceneManager.Instance.ActiveScene.UIScene.RenderScale = 1f;
            //             break;
            //         case >= 720:
            //             SceneManager.Instance.ActiveScene.UIScene.RenderScale = 0.75f;
            //             break;
            //         default:
            //             SceneManager.Instance.ActiveScene.UIScene.RenderScale = 0.5f;
            //             break;
            //     }
            // }
            // else
            // {
            //     SceneManager.Instance.ActiveScene.UIScene.RenderScale = this.Settings.ResolutionScale;
            // }

        }

        public override void LateUpdate()
        {
            base.LateUpdate();

            SceneManager.Instance.ActiveScene.LateUpdate();
        }
    }
}
