using Bean.Debug;
using Bean.PhysicsSystem;
using Bean.Player;
using Bean.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Bean.Graphics;

namespace Bean
{
    public class Engine : Game
    {
        private GraphicsDeviceManager _graphics;

        private SpriteBatch _spriteBatch;

        public bool GameLoopPause = false;

        public bool HasStarted = false;

        public static Engine Instance;

        public Vector2 UIPosition;

        private Timer _drawTimer;

        public Engine()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.SynchronizeWithVerticalRetrace = false;

            IsFixedTimeStep = true;

            TargetElapsedTime = TimeSpan.FromMilliseconds(5);

            Instance = this;

            Window.AllowUserResizing = true;

            this._drawTimer = new Timer();

            this._drawTimer.StartTimer(0);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (this._drawTimer.IsFinished)
            {
                this.Draw(_spriteBatch);
                this._drawTimer.StartTimer((float)GraphicsManager.Instance.TargetFPSMs / 1000);
                Time.Instance.Draw();
            }

            if (SceneManager.Instance.ActiveScene != null)
            {
                Texture2D cameraTex = SceneManager.Instance.ActiveScene.Camera.CameraTarget;
                Texture2D uITex = SceneManager.Instance.ActiveScene.UIScene.GetUITexture(this._spriteBatch);

                Vector2 scale = GraphicsManager.Instance.GetCurentWindowSize() / new Vector2(cameraTex.Width, cameraTex.Height);

                if (uITex == null || cameraTex == null)
                    return;

                this._spriteBatch.Begin();

                if (!this.GameLoopPause)
                    this._spriteBatch.Draw(cameraTex, Vector2.Zero, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
                else
                    this._spriteBatch.Draw(cameraTex, new Vector2(GraphicsManager.Instance.GetCurentWindowSize().X / 2, GraphicsManager.Instance.GetCurentWindowSize().Y / 2), null, Color.White, 0, new Vector2(cameraTex.Width / 2, cameraTex.Height / 2), 1, SpriteEffects.None, 0f);


                this._spriteBatch.Draw(uITex, new Vector2(GraphicsManager.Instance.GetCurentWindowSize().X / 2, GraphicsManager.Instance.GetCurentWindowSize().Y / 2), null, Color.White, 0, new Vector2(uITex.Width / 2, uITex.Height / 2), 1, SpriteEffects.None, 0f);

                GraphicsManager.Instance.GraphicsDevice.SetRenderTarget(null);

                this._spriteBatch.End();

                this.UIPosition = new Vector2(GraphicsManager.Instance.GetGameResolution().X / 2, GraphicsManager.Instance.GetGameResolution().Y / 2) - new Vector2(uITex.Width / 2, uITex.Height / 2);
            }


            Process currProcess = Process.GetCurrentProcess();
            long bytesUsed = currProcess.WorkingSet64 / 1000000;


            Window.Title = $"B.E.A.N | Update FPS: {Time.Instance.UpdateFps} ({Time.Instance.UpdateFrameTime}ms) | Draw FPS: {Time.Instance.DrawFps} ({Time.Instance.DrawFrameTime}ms) | Ram: {bytesUsed}Mb";
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {


        }

        protected override void Initialize()
        {
            DebugManager.Instance.Start();
            
            DebugServer.Log("Initializing Engine", this);
            base.Initialize();

            Window.TextInput += InputManager.Instance.ReciveOSKeyPress;
        }

        protected override void LoadContent()
        {
            DebugServer.Log("Creating Sprite Batch", this);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            DebugServer.Log("Creating Project Folders", this);
            FileManager.CreateProjectFolders();

            Globals.Content = Content;
            
            DebugServer.Log("Initializing Graphics Manager", this);
            GraphicsManager.Instance.StartGame(this.GraphicsDevice, this._graphics);
            GraphicsManager.Instance.SetFullScreen(false);
            GraphicsManager.Instance.SetGameResolution(1280, 720);
            GraphicsManager.Instance.ApplyChanges();


            this.Open();
            this.Load();
        }

        public virtual void Load()
        {

        }

        public virtual void LateUpdate()
        {

        }

        public virtual void Open()
        {

        }
        

        protected override void OnExiting(object sender, ExitingEventArgs args)
        {
            SceneManager.Instance.UnloadAllScenes();
            
            FileManager.UnloadAllAssets();
            
            
            base.OnExiting(sender, args);

            Environment.Exit(0);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //this.GameLoopPause = !this.IsActive;

            this._drawTimer.Update();

            DebugManager.Instance.Update();

            InputManager.Instance.Update();

            SceneManager.Instance.ActiveScene.Camera.Update();

            if(this.GameLoopPause && HasStarted)
                return;

            this.Update();

            Physics.Instance.Update();
                
            Time.Instance.Update(gameTime);

            this.LateUpdate();

            this.HasStarted = true;

        }

        public virtual void Update()
        {
        }

    }
}