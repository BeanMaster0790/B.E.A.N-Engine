using Bean.Scenes;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bean.UI;

public class UIText : ScreenProp
{
    public UIText(string name) : base(name)
    {
    }

    public string FontPath 
        {  
            get 
            { 
                return this._fontPath;  
            } 
            set 
            { 
                this._fontPath = value;
                this._fontSysem = FileManager.LoadFromFile<FontSystem>(this._fontPath);
            } 
        }

        public int FontSize = 18;

        public string Text = "New Text.";

        private string _fontPath;

        protected FontSystem _fontSysem;


    public override void Start()
    {
        base.Start();

        if(this._fontPath == null){
            this.FontPath = "EngineFont";
        }
    }
 
    public override void Update()
    {
        base.Update();

        this.Width = this._fontSysem.GetFont(this.FontSize).MeasureString(Text).X;
        this.Height = this._fontSysem.GetFont(this.FontSize).MeasureString(Text).Y;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        if(this._fontSysem == null)
            return;

        DynamicSpriteFont font = this._fontSysem.GetFont(this.FontSize * this._UIScene.RenderScale);

        if(this.Parent == null)
            spriteBatch.DrawString(font, Text, this.WorldPosition, this.Colour, layerDepth: this.Layer);
        else
            spriteBatch.DrawString(font, Text, this.LocalPosition, this.Colour, layerDepth: this.Layer);
    }
}
