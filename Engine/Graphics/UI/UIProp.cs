using System.Buffers;
using System.Security.Cryptography.X509Certificates;
using Bean.Graphics;
using Bean.Player;
using Bean.Scenes;
using Bean.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bean.UI
{
    public class ScreenProp : Prop
    {
        internal UIScene _UIScene;

        public ScreenProp Parent;

        public float Width;

        public float Height;

        public Color Colour;

        public float Layer = 0.5f;

        private bool? _localVisible = null;
        public override bool IsVisable
        {
            get
            {
                if (Parent != null)
                {
                    if (!Parent.IsVisable)
                        return false;

                    return _localVisible ?? true;
                }

                return _localVisible ?? true;
            }
            set
            {
                _localVisible = value;
            }
        }

        private bool? _localActive = null;
        public override bool IsActive
        {
            get
            {
                if (Parent != null)
                {
                    if (!Parent.IsActive)
                        return false;

                    return _localActive ?? true;
                }

                return _localActive ?? true;
            }
            set
            {
                _localActive = value;
            }
        }


    public UIPropAnchorPoint AnchorPoint = UIPropAnchorPoint.TopLeft;

    public Vector2 WorldPosition
    {
        get
        {
            if (this._UIScene == null)
                return this.LocalPosition;

            Vector2 screenAnchor = Vector2.Zero;

            switch (AnchorPoint)
            {
                case UIPropAnchorPoint.TopLeft:
                    screenAnchor = new Vector2(0, 0);
                    break;

                case UIPropAnchorPoint.TopCenter:
                    screenAnchor = new Vector2(_UIScene.RefernceWidth / 2f, 0);
                    break;

                case UIPropAnchorPoint.TopRight:
                    screenAnchor = new Vector2(_UIScene.RefernceWidth, 0);
                    break;

                case UIPropAnchorPoint.CenterLeft:
                    screenAnchor = new Vector2(0, _UIScene.RefrenceHeight / 2f);
                    break;

                case UIPropAnchorPoint.Center:
                    screenAnchor = new Vector2(_UIScene.RefernceWidth / 2f, _UIScene.RefrenceHeight / 2f);
                    break;

                case UIPropAnchorPoint.CenterRight:
                    screenAnchor = new Vector2(_UIScene.RefernceWidth, _UIScene.RefrenceHeight / 2f);
                    break;

                case UIPropAnchorPoint.BottomLeft:
                    screenAnchor = new Vector2(0, _UIScene.RefrenceHeight);
                    break;

                case UIPropAnchorPoint.BottomCenter:
                    screenAnchor = new Vector2(_UIScene.RefernceWidth / 2f, _UIScene.RefrenceHeight);
                    break;

                case UIPropAnchorPoint.BottomRight:
                    screenAnchor = new Vector2(_UIScene.RefernceWidth, _UIScene.RefrenceHeight);
                    break;
            }

            Vector2 propOffset = Vector2.Zero;
            switch (AnchorPoint)
            {
                case UIPropAnchorPoint.TopCenter:
                case UIPropAnchorPoint.Center:
                case UIPropAnchorPoint.BottomCenter:
                    propOffset.X = -Width * this._UIScene.RenderScale / 2f;
                    break;

                case UIPropAnchorPoint.TopRight:
                case UIPropAnchorPoint.CenterRight:
                case UIPropAnchorPoint.BottomRight:
                    propOffset.X = -Width * this._UIScene.RenderScale;
                    break;
            }

            switch (AnchorPoint)
            {
                case UIPropAnchorPoint.CenterLeft:
                case UIPropAnchorPoint.Center:
                case UIPropAnchorPoint.CenterRight:
                    propOffset.Y = -Height * this._UIScene.RenderScale / 2f;
                    break;

                case UIPropAnchorPoint.BottomLeft:
                case UIPropAnchorPoint.BottomCenter:
                case UIPropAnchorPoint.BottomRight:
                    propOffset.Y = -Height * this._UIScene.RenderScale;
                    break;
            }

            Vector2 newPos = screenAnchor + this.LocalPosition + propOffset;

            if (this.Parent != null)
                newPos += this.Parent.WorldPosition;

            return newPos;
        }
    }
        public Vector2 LocalPosition;
        public EventHandler OnHover;
        public EventHandler OnHoverEnter;
        public EventHandler OnHoverExit;
        public EventHandler OnLeftClick;
        public EventHandler OnRightClick;
        public EventHandler OnLeftClickHold;
        public EventHandler OnRightClickHold;
        public EventHandler OnDestroy;

        public bool IsSelected;

        private bool _isHovering;

        public RenderTarget2D target2D;

        public SoundHolder soundHolder;

        public ScreenProp(string name) : base(name)
        {
        }

        public override void Start()
        {
            base.Start();
        }

        public void AddSoundHolder()
        {
            this.soundHolder = new SoundHolder("SoundHolder");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (Engine.Instance.GameLoopPause)
                Update();
        }

        public override void Destroy()
        {
            base.Destroy();
            
            OnDestroy?.Invoke(this, EventArgs.Empty);
        }

        public override void Update()
        {
            base.Update();

            if (soundHolder != null)
                this.soundHolder.Update();
        }

        public override void LateUpdate()
        {
            base.LateUpdate();

            if (IsMouseOverProp() && this.IsVisable)
            {
                this.OnHover?.Invoke(this, null);

                if (!this._isHovering)
                {
                    this.OnHoverEnter?.Invoke(this, null);    
                }

                this._isHovering = true;

                if (InputManager.Instance.WasLeftButtonPressed())
                {
                    this.IsSelected = true;
                    this.OnLeftClick?.Invoke(this, null);
                }

                if (InputManager.Instance.WasRightButtonPressed())
                    this.OnRightClick?.Invoke(this, null);

                if (InputManager.Instance.IsRightButtonHeld())
                    this.OnRightClickHold?.Invoke(this, null);

                if (InputManager.Instance.IsLeftButtonHeld())
                    this.OnLeftClickHold?.Invoke(this, null);
            }
            else if (this.IsSelected)
            {
                if (InputManager.Instance.WasLeftButtonPressed())
                {
                    this.IsSelected = false;
                }
            }

            if (!IsMouseOverProp() && this._isHovering && this.IsVisable)
            {
                this.OnHoverExit?.Invoke(this, null);

                this._isHovering = false;
            }
        }

        public bool IsMouseOverProp()
        {
            Vector2 pos = this.WorldPosition + Engine.Instance.UIPosition;

            Vector2 mousePos = InputManager.Instance.MousePosition();

            bool xOverlap = false;
            bool yOverlap = false;

            if (mousePos.X > pos.X && mousePos.X < pos.X + Width * this._UIScene.RenderScale)
                xOverlap = true;

            if (mousePos.Y > pos.Y && mousePos.Y < pos.Y + Height * this._UIScene.RenderScale)
                yOverlap = true;

            return xOverlap && yOverlap;
        }

        public virtual void DrawRenderTarget(SpriteBatch spriteBatch)
        {
            return;
        }
    }

    public enum UIPropAnchorPoint
    {
        TopLeft,
        TopCenter,
        TopRight,
        CenterLeft,
        Center,
        CenterRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }
}