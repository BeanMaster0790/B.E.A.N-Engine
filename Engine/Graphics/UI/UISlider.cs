using System;
using Bean.Debug;
using Bean.Player;
using Microsoft.Xna.Framework;

namespace Bean.UI
{
    public class UISlider : UIAlignContainer
    {
        public Color FillColour = Color.White;

        private float _fillValue = 100f;

        private float _maxFill = 100f;

        public float MaxFill 
        {
            get { return _maxFill; }
            set
            {
                if (this._maxFill != value && this.FillContainer != null)
                    this.FillContainer._colourChanged = true;

                this._maxFill = value;
            }
        }

        public float FillValue
        {
            get { return _fillValue; }
            set
            {
                if (this._fillValue != value && this.FillContainer != null)
                    this.FillContainer._colourChanged = true;

                this._fillValue = Math.Clamp(value, 1f, this.MaxFill);

            }
        }

        public string HandleTexturePath;

        public int HandleWidth;

        public int HandleHeight;

        public bool IsInteractable;

        public UISliderFillDirection FillDirection = UISliderFillDirection.LeftRight;

        public UIContainer FillContainer;

        public UIContainer Handle;

        public override void Start()
        {
            base.Start();

            this.Layer = 0.9f;
            this.ChildrenLayer = 0.8f;

            this.FillContainer = new UIContainer()
            {
                Height = this.Height,
                Width = this.Width,
                Colour = new Color(172,50,50),
                Parent = this,
                Layer = 0f
            };

            this._UIScene.AddUIProp(this.FillContainer);

            if (!string.IsNullOrEmpty(this.HandleTexturePath))
            {
                this.Handle = new UIContainer()
                {
                    Width = this.HandleWidth,
                    Height = this.HandleHeight,
                    TexturePath = this.HandleTexturePath,
                    Layer = 1
                };

                this._UIScene.AddUIProp(this.Handle);
            }

            if (this.IsInteractable)
            {
                this.OnLeftClickHold += (object sender, EventArgs e) =>
                {
                    float diffrence = InputManager.Instance.MousePosition().X - this.WorldPosition.X;

                    this.FillValue = (int)((diffrence / (this.Width * this._UIScene.RenderScale)) * this.MaxFill);
                };
            }


            if (this.FillDirection == UISliderFillDirection.LeftRight)
                this.HorizontalAlign = HorizontalAlign.Left;
        }

        public override void Update()
        {
            base.Update();

            this.FillContainer.Height = this.Height;

            this.FillContainer.Width = this.Width * (this.FillValue / this.MaxFill);

            this.FillContainer.Width = Math.Clamp(this.FillContainer.Width, 1, float.MaxValue);

            if (this.Handle != null)
            {
                this.Handle.LocalPosition = new Vector2(this.WorldPosition.X + (this.Width * this._UIScene.RenderScale) * (this.FillValue / this.MaxFill) - (this.Handle.Width * this._UIScene.RenderScale / 2),
                this.WorldPosition.Y + (this.Height * this._UIScene.RenderScale / 2) - (this.Handle.Height * this._UIScene.RenderScale / 2));

                this.Handle.IsVisable = this.IsVisable;

                this.Handle.IsActive = this.IsActive;
            }
        }
    }

    public enum UISliderFillDirection
    {
        LeftRight,
        RightLeft,
    }
}