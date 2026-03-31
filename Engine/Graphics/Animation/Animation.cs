using Bean.Graphics.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bean.Graphics.Animations
{
    public class Animation
    {

        public string AnimationName;

        public int CurrentFrame { get; set; }

        public int FrameCount { get { return Frames.Count; } }

        public float FrameSpeed { get; set; }

        public List<Frame> Frames { get; set; }

        public bool IsLooping { get; set; }


        public Animation(string animationName)
        {
            this.Frames = new List<Frame>();

            this.AnimationName = animationName;

            this.IsLooping = true;

            this.FrameSpeed = 0.2f;
        }

        public Rectangle GetTextureRectangle()
        {
            Rectangle rect = new Rectangle(this.Frames[this.CurrentFrame].frame.x, this.Frames[this.CurrentFrame].frame.y, this.Frames[this.CurrentFrame].frame.w, this.Frames[this.CurrentFrame].frame.h);
            return rect;
        }

        public Vector2 GetCenterOrigin()
        {
            Vector2 pos = new Vector2(this.Frames[this.CurrentFrame].frame.w / 2, this.Frames[this.CurrentFrame].frame.h / 2);
            return pos;
        }

        public Vector2 GetFrameDimentions()
        {
            Vector2 dim = new Vector2(this.Frames[this.CurrentFrame].frame.w, this.Frames[this.CurrentFrame].frame.h);
            return dim;
        }

    }
}
