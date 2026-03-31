using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Bean.Graphics.Animation;
using Bean.Debug;
using Bean.JsonVariables;

namespace Bean.Graphics.Animations
{
    [RequiresAddon(typeof(Sprite))]
    public class AnimationManager : Addon, IJsonParsable<AnimationManager>
    {
        public Dictionary<string, Animation> Animations;

        [DebugServerVariable]
        public Animation CurrentAnimation;

        public EventHandler<FrameEventArgs> FrameEvent;

        private bool _isPlaying;

        private float _timer;

        private string _animationsDirectory;
        private string _texturePath;

        private Sprite _parentSprite;

        private string _constructorPath;

        private string _playOnceReady = "";

        public AnimationManager(string name, string SheetTexturePath) : base(name)
        {
            this._constructorPath = SheetTexturePath;
            this._texturePath = SheetTexturePath;
        }

        public void ChangeTexture(string SheetTexturePath)
        {
            this._parentSprite.ChangeTexture(SheetTexturePath);
            this._texturePath =  SheetTexturePath;

            this._animationsDirectory = FileManager.TexturePath + SheetTexturePath + ".json";

            if (!File.Exists(this._animationsDirectory))
            {
                return;
            }

            LoadAnimations();

            this._isPlaying = false;
            
            CurrentAnimation = this.Animations.First().Value;
        }

        public override void Start()
        {
            base.Start();
            
            this._parentSprite = this.Parent.GetAddon<Sprite>();
            
            ChangeTexture(this._constructorPath);
            
            if(!string.IsNullOrEmpty(this._playOnceReady))
                this.Play(this._playOnceReady);
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

        public override void LateUpdate()
        {
            base.LateUpdate();
            
            if(CurrentAnimation == null)
                return;
                
            this._parentSprite.ChangeSourceRect(CurrentAnimation.GetTextureRectangle());
            //this._parentSprite.ChangeOrigin(CurrentAnimation.GetCenterOrigin());
        }

        public void Play(string animation)
        {
            if(animation == "None")
            {
                this.CurrentAnimation = null;
                return;
            }

            if (this._parentSprite == null || this.Animations == null)
            {
                this._playOnceReady = animation;
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

        public struct AnimationManagerJson : IBeanJson
        {
            public string Name { get; set; }

            public string AnimationPath { get; set; }
        }

        public static AnimationManager Parse(string json)
        {
            AnimationManagerJson? animationManagerJsonNull = JsonConvert.DeserializeObject<AnimationManagerJson>(json);

            if (animationManagerJsonNull == null)
                throw new ArgumentException("AnimationManagerJson Is Null");
            
            AnimationManagerJson animationsJson = (AnimationManagerJson)animationManagerJsonNull;

            return new AnimationManager(animationsJson.Name, animationsJson.AnimationPath);
        }

        public void UpdateFromJson(string json)
        {
            AnimationManagerJson? animationManagerJsonNull = JsonConvert.DeserializeObject<AnimationManagerJson>(json);

            if (animationManagerJsonNull == null)
                throw new ArgumentException("AnimationManagerJson Is Null");
            
            AnimationManagerJson animationsJson = (AnimationManagerJson)animationManagerJsonNull;
            
            this.Name = animationsJson.Name;
            ChangeTexture(animationsJson.AnimationPath);
        }

        public string ExportJson()
        {
            AnimationManagerJson json = new AnimationManagerJson()
            {
                Name = this.Name,
                AnimationPath = this._texturePath,
            };
            
            return JsonConvert.SerializeObject(json);
        }
    }

    public class FrameEventArgs : EventArgs
    {
        public string AnimationName;

        public int CurrentFrame;
    }
}
