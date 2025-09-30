using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Bean.Graphics.Animations;

namespace Bean
{
    public class Sprite : WorldProp
    {
        public AnimationManager AnimationManager;

        public Color Colour = Color.White;

        private Vector2 _origin;

        public Vector2 GetOrigin()
        {
            if (this._userChangedOrigin)
                return this._origin;

            if (this.AnimationManager != null && !this.AnimationManager.ToRemove)
                return this.GetAddon<AnimationManager>().CurrentAnimation.GetCenterOrigin();

            if (this.Texture == null)
                return Vector2.Zero;

            return new Vector2(this.Texture.Width / 2, this.Texture.Height / 2);
        }

		private bool _userChangedOrigin;
        
        public SpriteEffects spriteEffect;

        public Texture2D Texture;

        private string _texturePath;

        public string TexturePath 
        {  
            get 
            { 
                return this._texturePath;  
            } 
            set 
            { 
                this._texturePath = value;
                this.Texture = FileManager.LoadFromFile<Texture2D>(this._texturePath);
            } 
        }

        public Rectangle GetSpriteRectangle()
        {
            if (this.AnimationManager != null)
            {
                Vector2 widthAndHeight = this.AnimationManager.CurrentAnimation.GetFrameDimentions();
                return new Rectangle((int)(Position.X - GetOrigin().X * this.Scale), (int)(Position.Y - GetOrigin().Y * this.Scale), (int)(widthAndHeight.X * this.Scale), (int)(widthAndHeight.Y * this.Scale));
            }

            if (Texture != null)
                return new Rectangle((int)(Position.X - GetOrigin().X * this.Scale), (int)(Position.Y - GetOrigin().Y * this.Scale), (int)(this.Texture.Width * this.Scale), (int)(this.Texture.Height * this.Scale));
            else
                return Rectangle.Empty;
        }

        public Sprite() : base()
        {
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (this.AnimationManager != null)
            {
                this.AnimationManager.Draw(spriteBatch);
            }

            else if (this.Texture != null)
                spriteBatch.Draw(this.Texture, this.Position - base.Scene.Camera.Position, null, this.Colour, MathHelper.ToRadians(this.Rotation), this.GetOrigin(), this.Scale, this.spriteEffect, this.Layer);
		}

        public override void Start()
        {
            base.Start();
        }

        public void SetOrigin(Vector2 origin)
        {
            this._origin = origin;
            this._userChangedOrigin = true;
        }

        public override void AddAddon(Addon addon)
        {
            base.AddAddon(addon);

            if(addon is AnimationManager animation)
                this.AnimationManager = animation;
        }

	}
}
