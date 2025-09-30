using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Bean.Graphics.Animation;
using System.IO;

namespace Bean.Graphics.Animations
{
    public class AnimationManager : Addon
    {
        public Dictionary<string, Animation> Animations;

        [DebugServerVariable]
        public Animation CurrentAnimation;

        public EventHandler<FrameEventArgs> FrameEvent;

        private bool _isPlaying;

        private float _timer;

        private string _animationsDirectory;

        public Texture2D Texture;

        private Sprite _parentSprite;

        public AnimationManager(string SheetTexturePath) : base()
        {
            ChangeTexture(SheetTexturePath);
        }

        public void ChangeTexture(string SheetTexturePath)
        {
            this.Texture = FileManager.LoadFromFile<Texture2D>(SheetTexturePath);

            this._animationsDirectory = FileManager.TexturePath + SheetTexturePath + ".json";

            LoadAnimations();

            this._isPlaying = false;
        }

        public override void Start()
        {
            if (this.Parent is Sprite sprite)
            {
                this._parentSprite = sprite;
            }
            else
            {
                throw new Exception("The AnimationManager cannot be added to an obect that doesn't inherit from Sprite");
            }

        }

        private void LoadAnimations()
        {
            this.Animations = new Dictionary<string, Animation>();

            string data = File.OpenText(this._animationsDirectory).ReadToEnd();

            ImageFrames imageFrames = JsonConvert.DeserializeObject<ImageFrames>(data);

            List<string> frameTags = new List<string>();

            foreach (FrameTag tag in imageFrames.meta.frameTags)
            {
                frameTags.Add(tag.name);

                Animation animation = new Animation(tag.name);

                this.Animations.Add(tag.name, animation);
            }

            foreach (Frame frame in imageFrames.frames)
            {
                string frameName = frame.filename.Split('#')[1];
                frameName = frameName.Split('.')[0];

                if (frameName.Contains(" "))
                    frameName = frameName.Split(" ")[0];

                this.Animations[frameName].Frames.Add(frame);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(this._parentSprite == null)
                return;

            if(this.CurrentAnimation != null && this._isPlaying)
                spriteBatch.Draw(this.Texture, base.Parent.Position - base.Parent.Scene.Camera.Position,
                CurrentAnimation.GetTextureRectangle(),
                this._parentSprite.Colour, MathHelper.ToRadians(base.Parent.Rotation), this._parentSprite.GetOrigin(), base.Parent.Scale, this._parentSprite.spriteEffect, base.Parent.Layer);
        }

        public void Play(string animation)
        {
            if(animation == "None")
            {
                this.CurrentAnimation = null;
                return;
            }

            Animation animationToPlay = this.Animations[animation];

            if (this.CurrentAnimation == animationToPlay && this._isPlaying)
                return;

            this._isPlaying = true;

            this.CurrentAnimation = animationToPlay;
            this.CurrentAnimation.CurrentFrame = 0;

            this._timer = 0;
        }

        public void Pause()
        {
            this._isPlaying = false;
        }

        public void Resume()
        {
            this._isPlaying = true;
        }

        public void Stop()
        {
            this._isPlaying = false;

            this._timer = 0;
        }

        public override void Update()
        {
            base.Update();

            if (!this._isPlaying)
                return;

            this._timer += Time.Instance.DeltaTime;

            if (this._timer >= CurrentAnimation.FrameSpeed)
            {
                this._timer = 0;

                this.CurrentAnimation.CurrentFrame++;

                FrameEventArgs args = new FrameEventArgs();
                args.AnimationName = this.CurrentAnimation.AnimationName;
                args.CurrentFrame = this.CurrentAnimation.CurrentFrame;

                if (this.CurrentAnimation.CurrentFrame >= this.CurrentAnimation.FrameCount && this.CurrentAnimation.IsLooping)
                {
                    this.CurrentAnimation.CurrentFrame = 0;
                }
                else if (this.CurrentAnimation.CurrentFrame >= this.CurrentAnimation.FrameCount)
                {
                    this.CurrentAnimation.CurrentFrame = 0;
                    Stop();
                }
                

                FrameEvent?.Invoke(this, args);
            }
        }

    }

    public class FrameEventArgs : EventArgs
    {
        public string AnimationName;

        public int CurrentFrame;
    }
}
