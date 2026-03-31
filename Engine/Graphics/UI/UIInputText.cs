using System.Buffers;
using Bean.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bean.UI
{
    public class UIInputText : UIText
    {
        public enum TextInputType
        {
            None,
            Alpha,
            AlphaSymbols,
            Int,
            Decimal,
        }
        
        public TextInputType InputType = TextInputType.None;
        
        public string InputtedText;
        public string PlaceHolderText;
        
        private UIContainer _cursor;

        private int _cursorIndex;

        public EventHandler OnTextInput;

        public UIInputText(string name) : base(name)
        {
        }

        public override void Start()
        {
            base.Start();

            InputManager.Instance.TextInput += WriteText;

            _cursor = new UIContainer("Cursor");

            _cursor.Width = 2;
            _cursor.Layer = 1;
            _cursor.Height = this._fontSysem.GetFont(this.FontSize).MeasureString("I").Y;

            _cursor.LocalPosition = new Vector2(0,0);

            _cursor.Parent = this;

            _cursor.Colour = Color.White;

            this._UIScene.AddUIProp(_cursor);

            this.OnLeftClick += (object sender, EventArgs args) => 
            {
                if(string.IsNullOrEmpty(this.InputtedText))
                    return;

                Vector2 pos = this.WorldPosition + Engine.Instance.UIPosition;

                Vector2 mousePos = InputManager.Instance.MousePosition();

                for (int i = 0; i < this.InputtedText.Length + 1; i++)
                {
                    string MeasureString = this.InputtedText.Remove(i);

                    float Width = this._fontSysem.GetFont(this.FontSize * this._UIScene.RenderScale).MeasureString(MeasureString).X;

                    Vector2 charPos = new Vector2(pos.X + Width, pos.Y + this._fontSysem.GetFont(this.FontSize * this._UIScene.RenderScale).MeasureString("I").Y);

                    if(mousePos.X < charPos.X)
                    {
                        this._cursorIndex = i;
                        break;
                    }
                }

            };
        }

        public override void Update()
        {
            base.Update();

            if(this.Parent != null)
            {
                this.IsSelected = this.Parent.IsSelected;
            }

            if (string.IsNullOrEmpty(this.InputtedText) && !this.IsSelected)
            {
                this.Text = this.PlaceHolderText;
                this._cursor.IsVisable = false;
            }
            else
            {
                this.Text = this.InputtedText;
                this._cursor.IsVisable = true;
            }

            if (!IsSelected)
            {
                this._cursor.IsVisable = false;
                this._cursorIndex = this.InputtedText.Length;
                return;
            }

            if (InputManager.Instance.WasKeyPressed(Keys.Right))
            {
                this._cursorIndex += 1;
            }
            else if (InputManager.Instance.WasKeyPressed(Keys.Left))
            {
                this._cursorIndex -= 1;
            }

            if (string.IsNullOrEmpty(InputtedText))
                return;

            this._cursorIndex = MathHelper.Clamp(this._cursorIndex, 0, this.InputtedText.Length);
            
            ScrollWithCursor();

        }

        private void ScrollWithCursor()
        {
            string indexMesureString = this.InputtedText.Remove(this._cursorIndex);

            float currentCharPosition = this._fontSysem.GetFont(this.FontSize * this._UIScene.RenderScale).MeasureString(indexMesureString).X;

            this._cursor.LocalPosition.X = currentCharPosition;

            if(!IsCharFullyVisable(currentCharPosition) && this.Parent is UIAlignContainer alignContainer)
            {
                if(currentCharPosition - alignContainer._scroll < 0)
                {
                    float scrollAdjustment = currentCharPosition - alignContainer._scroll;

                    alignContainer._scroll += scrollAdjustment;
                }
                else
                {
                    float scrollAdjustment = currentCharPosition - (alignContainer._scroll + alignContainer.Width * this._UIScene.RenderScale);

                    alignContainer._scroll += scrollAdjustment;
                }

            }
        }

        private bool IsCharFullyVisable(float charXPos)
        {
            if(this.Parent is UIAlignContainer alignContainer)
            {
                if(charXPos <= alignContainer._scroll + alignContainer.Width * this._UIScene.RenderScale && charXPos - alignContainer._scroll >= 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private void WriteText(object sender, TextInputEventArgs args)
        {
            if(!IsSelected)
                return;

            string lastChar = args.Character.ToString();

            if(string.IsNullOrEmpty(lastChar))
                return;

            string lowerChar = lastChar.ToLower();

            switch (lowerChar)
            {
                case "\b":
                    if(!string.IsNullOrEmpty(this.InputtedText) && this._cursorIndex > 0)
                    {
                        this.InputtedText = this.InputtedText.Remove(_cursorIndex - 1, 1);
                        this._cursorIndex--;
                        
                        this.OnTextInput?.Invoke(this, EventArgs.Empty);
                    }

                    break;
                
                default:

                    if (this.InputType == TextInputType.Alpha)
                    {
                        if(!char.IsLetter(lastChar[0]))
                            return;
                    }
                    else if(this.InputType == TextInputType.AlphaSymbols)
                    {
                        if(!char.IsLetter(lastChar[0]) && !char.IsSymbol(lastChar[0]))
                            return;
                    }
                    else if(this.InputType == TextInputType.Int)
                    {
                        if(!char.IsDigit(lastChar[0]) && lastChar[0] != '-')
                            return;
                    }
                    else if (this.InputType == TextInputType.Decimal)
                    {
                        if(!char.IsDigit(lastChar[0]) && (lastChar[0] != '.') && lastChar[0] != '-')
                            return;
                    }
                    
                    
                    if(string.IsNullOrEmpty(this.InputtedText))
                        this.InputtedText += lastChar;
                    else
                        this.InputtedText = this.InputtedText.Insert(_cursorIndex, lastChar);

                    this._cursorIndex++;
                    
                    this.OnTextInput?.Invoke(this, EventArgs.Empty);
                        
                    break;
            }
        }
    }
}