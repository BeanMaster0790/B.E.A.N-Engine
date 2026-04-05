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

            Scene scene = new Scene("PlanetScene");
            
            //scene.Camera.EnableLighting = true;
            scene.Camera.SetZ(scene.Camera.GetZFromHeight(200));
            
            scene.LightingManager.GlobalColour = new Color(0.3f, 0.3f, 0.3f);
            
            SceneManager.Instance.AddNewScene(scene);
            
            SceneManager.Instance.LoadScene("PlanetScene");
            
            SceneManager.Instance.SetActiveScene("PlanetScene");

            scene.AddToScene(FileManager.LoadWorldPropFromFile("Player"));
            
            //test.GetAddon<Sprite>().UpdateFromJson(test.GetAddon<Sprite>().ExportJson());
        }

        public override void Update()
        { 
            SceneManager.Instance.ActiveScene.Update();
        }

        public override void LateUpdate()
        {
            base.LateUpdate();

            SceneManager.Instance.ActiveScene.LateUpdate();
        }
    }
}
