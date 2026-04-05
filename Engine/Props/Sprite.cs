using Bean.Debug;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Bean.Graphics.Animations;
using Bean.JsonVariables;
using Newtonsoft.Json;

namespace Bean
{
    public class Sprite : Addon
    {
        [Tinned("Colour", customParseType: typeof(JsonColourConverter))]
        public Color Colour = Color.White;

        [Tinned("Origin")]
        public Vector2 Origin { get; private set; }
        
        [Tinned("SpriteEffect")]
        private SpriteEffects _spriteEffect;

        private Texture2D _texture;
        
        [Tinned("Texture", true, 1)]
        public string TexturePath;
        
        private Rectangle? _sourceRect;

        public Rectangle SpriteRectangle {get; private set;}
        
        private bool _ownTexture;

        public Sprite(string name, string texturePath) : base(name)
        {
            if(string.IsNullOrEmpty(texturePath))
                return;
            
            ChangeTexture(texturePath);
            ChangeOrigin(new Vector2((float)this._texture.Width / 2, (float)this._texture.Height / 2));
            ChangeSourceRect(null);
            ChangeSpriteEffect(SpriteEffects.None);
        }

        [Refry("Texture")]
        public void ChangeTexture(string texturePath)
        {
            Console.WriteLine("ChangingTexture");
            
            this._texture = FileManager.LoadFromFile<Texture2D>(texturePath);
            _ownTexture = false;
            this.TexturePath =  texturePath;
        }

        public void ChangeTexture(Texture2D texture)
        {
            this._texture = texture;
            _ownTexture = true;
        }
        
        public void ChangeOrigin(Vector2 origin)
        {
            this.Origin = origin;
        }

        public void ChangeSourceRect(Rectangle? sourceRect)
        {
            this._sourceRect = sourceRect;
        }

        public void ChangeSpriteEffect(SpriteEffects spriteEffect)
        {
            this._spriteEffect = spriteEffect;
        }

        public void DisposeTexture()
        {
            if (!this._ownTexture)
            {
                DebugServer.LogWarning("You tried disposing a texture which belongs to the AssetManager. Ignoring request to avoid errors!", this);
                return;
            }
            
            this._texture.Dispose();
            this._texture = null;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            
            if(_texture == null)
                return;
            
            spriteBatch.Draw(this._texture, this.Parent.PropTransform.GetDrawPosition(), this._sourceRect, this.Colour, MathHelper.ToRadians(this.Parent.PropTransform.Rotation),
                this.Origin, this.Parent.PropTransform.Scale, this._spriteEffect, this.Parent.PropTransform.Layer);
		}

        public override void LateUpdate()
        {
            base.LateUpdate();
            
            if(_texture == null)
                return;
            
            if(this._sourceRect == null)
                this.SpriteRectangle = new Rectangle((int)this.Parent.PropTransform.Position.X,  (int)this.Parent.PropTransform.Position.Y, 
                    (int)(this._texture.Width * this.Parent.PropTransform.Scale.X), (int)(this._texture.Height * this.Parent.PropTransform.Scale.Y));
            else
                this.SpriteRectangle = new Rectangle((int)this.Parent.PropTransform.Position.X,  (int)this.Parent.PropTransform.Position.Y, 
                    (int)(this._sourceRect.Value.Width * this.Parent.PropTransform.Scale.X), (int)(this._sourceRect.Value.Height * this.Parent.PropTransform.Scale.Y));
        }
    }
}
