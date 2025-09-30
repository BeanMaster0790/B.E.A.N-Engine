using System;
using Bean;
using Bean.Graphics;
using Bean.Player;
using Bean.Scenes;
using Bean.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DemoGame
{
    public class GraphicSettingsMenu : Menu
    {

        public UIAlignContainer TitleCard;

        public UIAlignContainer ResolutionCard;

        public UIAlignContainer UiScaleCard;

        public UIAlignContainer FpsCard;

        public UIAlignContainer BackCard;

        public GraphicSettingsMenu(Scene scene, string key) : base(scene, key)
        {
        }

        public override void Start()
        {
            base.Start();

            this.TitleCard = CreateCard("Video", "UI/BigUIBox", "TitleCard");
            this.ResolutionCard = CreateCard("Window", "UI/MedUIBox", "ResolutionCard");
            this.UiScaleCard = CreateCard("UI Scale", "UI/MedUIBox", "UIScaleCard");
            this.FpsCard = CreateCard("FPS", "UI/MedUIBox", "FpsCard");
            this.BackCard = CreateCard("Back", "UI/MedUIBox", "BackCard");

            this.BackCard.OnLeftClick += (object sender, EventArgs e) =>
            {
                this.MenuHandler.OpenMenu("Settings");
            };

            this.ResolutionCard.OnLeftClick += (object sender, EventArgs e) =>
            {
                this.MenuHandler.OpenMenu("Resolution");
            };

            this.FpsCard.OnLeftClick += (object sender, EventArgs e) =>
            {
                this.MenuHandler.OpenMenu("FPS");
            };

            this.UiScaleCard.OnLeftClick += (object sender, EventArgs e) =>
            {
                this.MenuHandler.OpenMenu("Scale");
            };
        }

        public UIAlignContainer CreateCard(string text, string texturePath, string name)
        {
            UIAlignContainer card = new UIAlignContainer()
            {
                TexturePath = texturePath,
                Name = name,
                Width = 600,
                Height = 200,
                HorizontalAlign = HorizontalAlign.Center,
                VerticalAlign = VerticalAlign.Center,
                Parent = this
            };

            UIText cardText = new UIText()
            {
                LocalPosition = Vector2.Zero,
                Colour = Color.White,
                Width = 100,
                Height = 50,
                FontSize = 64,
                Name = name + "Text",
                FontPath = "GameFont1",
                Text = text,
                Parent = card
            };

            if (!name.Contains("Title"))
            {

                card.soundHolder.AddSound(this._UIScene.Scene.SoundManager, "Hover", "Sounds/UI1");
                card.soundHolder.AddSound(this._UIScene.Scene.SoundManager, "Click", "Sounds/UI3");

                card.OnHover += (object sender, EventArgs e) =>
                {
                    card.OverlayColour = Color.DimGray;
                };
                card.OnHoverEnter += (object sender, EventArgs e) =>
                {
                    card.soundHolder.PlaySound("Hover");
                };
                card.OnHoverExit += (object sender, EventArgs e) =>
                {
                    card.OverlayColour = Color.White;
                };
                card.OnLeftClick += (object sender, EventArgs e) =>
                {
                    card.soundHolder.PlaySound("Click");
                };
            }

            this._UIScene.AddUIProp(card);
            this._UIScene.AddUIProp(cardText);

            return card;
        }
    }
}