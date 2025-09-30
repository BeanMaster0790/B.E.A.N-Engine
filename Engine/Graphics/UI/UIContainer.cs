using System;
using System.Collections.Generic;
using System.Diagnostics;
using Bean.Graphics;
using Bean.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bean.UI
{
    public class UIContainer : ScreenProp
    {
        protected List<ScreenProp> _children 
        {
            get
            {
                return this._UIScene.SceneProps.FindAll(p => p.Parent == this);
            }
        }

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
                this._texture = FileManager.LoadFromFile<Texture2D>(this._texturePath);
            } 
        }
        
        private Texture2D _texture;

        public Vector2 MaskPosition;

        private float _lastWidth;

        private float _lastHeight;

        private float _lastScale;

        public Color OverlayColour = Color.White;

        public float ChildrenLayer = 1;

        public new Color Colour
        {
            get
            {
                return this._colour;
            }

            set
            {
                this._colour = value;
                this._colourChanged = true;
            }
        }

        private Color _colour; 

        internal bool _colourChanged;

        public override void Start()
        {
            base.Start();

            SetTexture();
        }

        private void SetTexture()
        {
            if (this._texture == null || (this._colourChanged && this._texture != null))
            {
                if (this.Colour == Color.Transparent)
                {
                    CustomShape transSquare = new CustomShape(colour: this.Colour);

                    transSquare.SetPoints(new List<Vector2>() { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) });

                    this._texture = transSquare.Texture;
                }
                else
                {
                    CustomShape square = new CustomShape(colour: this.Colour);

                    square.SetPoints(new List<Vector2>() { new Vector2(0, 0), new Vector2(this.Width, 0), new Vector2(this.Width, this.Height), new Vector2(0, this.Height) });

                    this._texture = square.Texture;
                }

            }

            this._colourChanged = false;
        }

        public override void Update()
        {
            base.Update();

            float scaleX = this.Width / (float)this._texture.Width;
            float scaleY = this.Height / (float)this._texture.Height;
            float scale = Math.Min(scaleX, scaleY);

            if (this._texturePath != null)
            {
                this.Width  = this._texture.Width * scale;
                this.Height = this._texture.Height * scale;    
            } 
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (this._colourChanged)
                SetTexture();

            if (this.target2D != null)
            {
                if (Parent == null)
                    spriteBatch.Draw(this.target2D, this.WorldPosition, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, this.ChildrenLayer);
                else
                    spriteBatch.Draw(this.target2D, this.LocalPosition, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, this.ChildrenLayer);
            }

            if (this._texture == null)
                return;

            if (Parent == null || !(Parent is UIContainer))
            {
                if (this._texturePath == null)
                    spriteBatch.Draw(this._texture, this.WorldPosition, null, this.OverlayColour, 0, Vector2.Zero, this._UIScene.RenderScale, SpriteEffects.None, this.Layer);
                else
                {
                    float scaleX = this.Width / (float)this._texture.Width;
                    float scaleY = this.Height / (float)this._texture.Height;
                    float scale = Math.Min(scaleX, scaleY);

                    spriteBatch.Draw(
                        this._texture,
                        this.WorldPosition,
                        null,
                        this.OverlayColour,
                        0,
                        Vector2.Zero,
                        scale * this._UIScene.RenderScale,
                        SpriteEffects.None,
                        this.Layer
                    );
                }
            }
            else
            {
                if (this._texture != null)
                {
                    if (this._texturePath == null)
                        spriteBatch.Draw(this._texture, this.LocalPosition, null, this.OverlayColour, 0, Vector2.Zero, this._UIScene.RenderScale, SpriteEffects.None, this.Layer);
                    else
                    {
                        float scaleX = this.Width / (float)this._texture.Width;
                        float scaleY = this.Height / (float)this._texture.Height;
                        float scale = Math.Min(scaleX, scaleY);

                        spriteBatch.Draw(
                            this._texture,
                            this.LocalPosition,
                            null,
                            this.OverlayColour,
                            0,
                            Vector2.Zero,
                            scale * this._UIScene.RenderScale,
                            SpriteEffects.None,
                            this.Layer
                        );
                    }
                }
            }  
        }

        public override void DrawRenderTarget(SpriteBatch spriteBatch)
        {
            base.DrawRenderTarget(spriteBatch);

            if(this._lastHeight != this.Height || this._lastWidth != this.Width || this._lastScale != this._UIScene.RenderScale)
            {

                float scaledWidth = this.Width * this._UIScene.RenderScale;
                float scaledHeight = this.Height * this._UIScene.RenderScale;

                this.target2D?.Dispose();
                this.target2D = new RenderTarget2D(GraphicsManager.Instance.GraphicsDevice, 
                                            (int)((scaledWidth < 1) ? 1 : scaledWidth) , 
                                            (int)((scaledHeight < 1) ? 1 : scaledHeight));

                this._lastHeight = this.Height;
                this._lastWidth = this.Width;
                this._lastScale = this._UIScene.RenderScale;
            }

            GraphicsManager.Instance.GraphicsDevice.SetRenderTarget(this.target2D);
            GraphicsManager.Instance.GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.FrontToBack);

            foreach (ScreenProp prop in this._children)
            {
                prop.Draw(spriteBatch);
            }

            spriteBatch.End();

            GraphicsManager.Instance.GraphicsDevice.SetRenderTarget(null);
        }

        public override void Destroy()
        {
            base.Destroy();

            foreach(ScreenProp prop in this._children)
            {
                this._UIScene.RemoveUIProp(prop);
            }
        }

    }
}