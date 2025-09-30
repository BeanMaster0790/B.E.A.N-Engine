using Bean.Debug;
using Microsoft.Xna.Framework;

namespace Bean
{
    public class Time
    {
        public float DeltaTime { get; private set; }

        public int UpdateFps { get; private set; }
        public int DrawFps { get; private set; }

        public int UpdateFrameTime { get; private set; }
        public int DrawFrameTime { get; private set; }

        public float TargetMultiplier { get; private set; } = 1f;

        public static Time Instance = new Time();

        private long _lastDrawMs = 0;
        private long _drawMs = 0;

        public float SecondsSinceStart { get; private set; }

        public GameTime GameTime;


        public void Update(GameTime gameTime)
        {
            GameTime = gameTime;
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            UpdateFrameTime = (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (UpdateFrameTime > 0)
            {
                UpdateFps = (int)(1000f / UpdateFrameTime);
            }

            if (UpdateFrameTime > 0 && UpdateFrameTime < 1000)
            {
                int targetMs = 5;
                TargetMultiplier = (float)UpdateFrameTime / targetMs;
            }
            else
            {
                DebugServer.Log("⚠️Game update running slowly or loading. Expect weird behaviour!⚠️", this, Color.Red);
            }
        }

        public void Draw()
        {
            if (GameTime == null)
                return;

            SecondsSinceStart = (float)GameTime.TotalGameTime.TotalSeconds;

            _lastDrawMs = _drawMs;
            _drawMs = (long)GameTime.TotalGameTime.TotalMilliseconds;

            long diff = _drawMs - _lastDrawMs;

            DrawFrameTime = (int)diff;

            if (diff > 0)
            {
                DrawFps = (int)(1000f / diff);
            }
        }
    }
}
