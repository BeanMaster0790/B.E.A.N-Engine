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
    public class FPSMenu : Menu
    {
        public FPSMenu(Scene scene, string key) : base(scene, key)
        {
        }

        public override void Start()
        {
            base.Start();

            CreateCard("Set FPS", "UI/BigUIBox", "FullscreenTitleCard", 0);
            CreateCard("30", "UI/MedUIBox", "FullscreenCard", 30);
            CreateCard("60", "UI/MedUIBox", "WindowCard", 60);
            CreateCard("90", "UI/MedUIBox", "WindowCard", 90);
            CreateCard("120", "UI/MedUIBox", "WindowCard", 120);
            CreateCard("200", "UI/MedUIBox", "WindowCard", 200);
        }

        private UIAlignContainer CreateCard(string text, string texturePath, string name, int fps)
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
                        game.Settings.FPS = fps;
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