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
    public class MainMenuList : Menu
    {

        public UIAlignContainer TitleCard;

        public UIAlignContainer PlayCard;

        public UIAlignContainer SettingsCard;

        public UIAlignContainer QuitCard;

        public MainMenuList(Scene scene, string key) : base(scene, key)
        {
        }

        public override void Start()
        {
            base.Start();

            this.PlayCard = CreateCard("Play", "UI/MedUIBox", "PlayCard");
            this.SettingsCard = CreateCard("Settings", "UI/MedUIBox", "SettingsCard");
            this.QuitCard = CreateCard("Quit", "UI/MedUIBox", "QuitCard");

            this.QuitCard.OnLeftClick += (object sender, EventArgs e) =>
            {
                Engine.Instance.Exit();
            };

            this.SettingsCard.OnLeftClick += (object sender, EventArgs e) =>
            {
                this.MenuHandler.OpenMenu("Settings");
            };

            this.PlayCard.OnLeftClick += (object sender, EventArgs e) =>
            {
                if (this.MenuHandler.Scene is MainMenu mainMenu)
                    mainMenu.StartGame = true;
                else if (this.MenuHandler.Scene is GameWorld demoWorld)
                    demoWorld.CloseMenu();
            };
        }

        public override void Update()
        {
            base.Update();

            if (InputManager.Instance.WasKeyPressed(Keys.Up))
                this._UIScene.RenderScale += 0.2f;
            else if (InputManager.Instance.WasKeyPressed(Keys.Down))
                this._UIScene.RenderScale -= 0.2f;
        }

        private UIAlignContainer CreateCard(string text, string texturePath, string name)
        {
            UIAlignContainer card = new UIAlignContainer()
            {
                TexturePath = texturePath,
                Name = name,
                Width = 600,
                Height = 200,
                HorizontalAlign = HorizontalAlign.Center,
                VerticalAlign = VerticalAlign.Center,
                Parent = this,
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