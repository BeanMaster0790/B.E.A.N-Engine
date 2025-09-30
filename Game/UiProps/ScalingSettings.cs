using System;
using System.Linq;
using Bean;
using Bean.Graphics;
using Bean.Player;
using Bean.Scenes;
using Bean.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DemoGame
{
    public class ScalingSettingsMenu : Menu
    {
        public ScalingSettingsMenu(Scene scene, string key) : base(scene, key)
        {
        }

        public override void Start()
        {
            base.Start();

            CreateCard("Set UI Scale", "UI/BigUIBox", "FullscreenTitleCard", 0);
            CreateCard("auto", "UI/MedUIBox", "FullscreenCard", 0);
            CreateCard("50%", "UI/MedUIBox", "WindowCard", 0.5f);
            CreateCard("75%", "UI/MedUIBox", "WindowCard", 0.75f);
            CreateCard("100%", "UI/MedUIBox", "WindowCard", 1f);
            CreateCard("125%", "UI/MedUIBox", "WindowCard", 1.25f);
            CreateCard("150%", "UI/MedUIBox", "WindowCard", 1.5f);
        }

        private UIAlignContainer CreateCard(string text, string texturePath, string name, float scale)
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
                card.OnLeftClick += (object sender, EventArgs e) =>
                {
                    if (Engine.Instance is Game game)
                    {
                        game.Settings.ResolutionScale = scale;
                        game.Settings.ApplySettings();
                    }

                    this.MenuHandler.OpenMenu("Video");
                };
            }

            this._UIScene.AddUIProp(card);
            this._UIScene.AddUIProp(cardText);

            return card;
        }
    }
}