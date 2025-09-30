using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Bean.Graphics
{
    public class GraphicsManager
    {
        public static GraphicsManager Instance = new GraphicsManager();

        public GraphicsDevice GraphicsDevice;

        public GraphicsDeviceManager GraphicsDeviceManager;

        public EventHandler<EventArgs> GraphicsChanged;

        public Vector2 UserFullscreenSize;

        public int TargetFPSMs = 5;

        public bool IsFullscreen { get { return this.GraphicsDeviceManager.IsFullScreen; } }

        private int screenWidth
        {
            get
            {
                return this.GraphicsDeviceManager.PreferredBackBufferWidth;
            }

        }

        private int screenHeight
        {
            get
            {
                return this.GraphicsDeviceManager.PreferredBackBufferHeight;
            }

        }

        public void ApplyChanges()
        {
            this.GraphicsDeviceManager.ApplyChanges();

            this.GraphicsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void StartGame(GraphicsDevice device, GraphicsDeviceManager deviceManager)
        {
            this.GraphicsDevice = device;
            this.GraphicsDeviceManager = deviceManager;

            UserFullscreenSize = this.GetUserDisplaySize();

            this.SetGameResolution(1280, 720);
            this.SetFullScreen(false);

            this.ApplyChanges();

            Engine.Instance.Window.ClientSizeChanged += ResizeDraggedWindow;
        }

        private static void ResizeDraggedWindow(object sender, EventArgs args)
        {
            if (!Engine.Instance.Window.AllowUserResizing)
                return;

            Engine.Instance.Window.ClientSizeChanged -= ResizeDraggedWindow;

            GraphicsManager.Instance.SetGameResolution(Engine.Instance.Window.ClientBounds.Width, Engine.Instance.Window.ClientBounds.Height);
            GraphicsManager.Instance.ApplyChanges();

            Engine.Instance.Window.ClientSizeChanged += ResizeDraggedWindow;
        }

        public void SetGameResolution(int width, int height)
        {
            this.GraphicsDeviceManager.PreferredBackBufferHeight = height;
            this.GraphicsDeviceManager.PreferredBackBufferWidth = width;
        }

        public Vector2 GetGameResolution()
        {
            return new Vector2(this.screenWidth, this.screenHeight);
        }

        private Vector2 GetUserDisplaySize()
        {
            int width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            return new Vector2(width, height);
        }

        public float GetGameAspectRatio()
        {
            Vector2 screenSize = GetGameResolution();

            return screenSize.X / screenSize.Y;
        }

        public float GetFullscreenAspectRatio()
        {
            Vector2 screenSize = this.UserFullscreenSize;

            return screenSize.X / screenSize.Y;
        }

        public Vector2 GetCurentWindowSize()
        {
            if (this.IsFullscreen)
                return this.UserFullscreenSize;

            return GetGameResolution();
        }

        public void SetFullScreen(bool state)
        {
            this.GraphicsDeviceManager.IsFullScreen = state;

            if(state == true)
                SetGameResolution((int)this.UserFullscreenSize.X, (int)this.UserFullscreenSize.Y);
            else
                SetGameResolution(1280, 720);
        }

        public void SetFPS(int fps)
        {
            int ms = (int)MathF.Floor(1000 / fps);

            this.TargetFPSMs = ms;
        }

    }
}
