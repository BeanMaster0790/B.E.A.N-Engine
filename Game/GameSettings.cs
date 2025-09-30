using System;
using System.IO;
using Bean.Debug;
using Bean.Graphics;
using Bean.Scenes;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace DemoGame
{

    public class GameSettingsData
    {
        public float ResolutionScale = 0;

        public bool Fullscreen = false;

        public int FPS = 200;
    }

    public class GameSettings
    {
        public float ResolutionScale;

        public bool Fullscreen;

        public int FPS;

        public GameSettings()
        {

            DebugServer.Log(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/B.E.A.N/DemoGame/Settings.json", this);

            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/B.E.A.N/DemoGame/Settings.json"))
            {
                GameSettingsData gameSettings = JsonConvert.DeserializeObject<GameSettingsData>(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/B.E.A.N/DemoGame/Settings.json"));

                this.ResolutionScale = gameSettings.ResolutionScale;

                this.Fullscreen = gameSettings.Fullscreen;

                this.FPS = gameSettings.FPS;

                ApplySettings();
            }
            else
            {
                this.ResolutionScale = 0;

                this.Fullscreen = false;

                this.FPS = 200;

                ApplySettings();
            }
        }

        public void ApplySettings()
        {

            GraphicsManager.Instance.SetFullScreen(this.Fullscreen);

            GraphicsManager.Instance.SetFPS(this.FPS);

            GraphicsManager.Instance.ApplyChanges();
            
            string json = JsonConvert.SerializeObject(this);

            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/B.E.A.N/DemoGame"))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/B.E.A.N/DemoGame");
            }

            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/B.E.A.N/DemoGame/Settings.json", json);
        }
    }
}