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
    public class UpgradeMenu : Menu
    {

        public UIAlignContainer TitleCard;

        public UIAlignContainer FireRate;

        public UIAlignContainer SpecialCard;

        public UIAlignContainer HealthCard;

        public UIAlignContainer BackCard;

        public UIAlignContainer CostHoverCard;

        public UIText CostHoverCardText;

        public UIAlignContainer DescHoverCard;

        public UIText DescHoverCardText;

        private float _fireRateIncrementAmount = 0.5f;
        private float _maxFireRate = 7;

        private int _maxHealthIncrementAmount = 25;
        private int _maxHealth = 225;

        public UpgradeMenu(Scene scene, string key) : base(scene, key)
        {
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Start()
        {
            base.Start();

            this.TitleCard = CreateCard("Upgrades", "UI/BigUIBox", "TitleCard");
            this.FireRate = CreateCard("Fire Rate", "UI/MedUIBox", "FireCard");
            this.SpecialCard = CreateCard("Special", "UI/MedUIBox", "SpecialCard");
            this.HealthCard = CreateCard("Max Health", "UI/MedUIBox", "HealthCard");
            this.BackCard = CreateCard("Back", "UI/MedUIBox", "BackCard");

            this.BackCard.OnLeftClick += (object sender, EventArgs e) =>
            {
                this.MenuHandler.OpenMenu("");

                this.DescHoverCard.IsVisable = false;
                this.CostHoverCard.IsVisable = false;
            };

            this.FireRate.OnLeftClick += (object sender, EventArgs e) =>
            {
                if (this._UIScene.Scene is GameWorld world)
                {
                    float currentFireRate = world.Player.Gun.FireRatePerSecond;

                    if (currentFireRate == this._maxFireRate)
                        return;

                    float newFireRate = currentFireRate + this._fireRateIncrementAmount;

                    int cost = 0;

                    switch (newFireRate)
                    {
                        case 5:
                            cost = 150;
                            break;
                        case 5.5f:
                            cost = 250;
                            break;
                        case 6:
                            cost = 400;
                            break;
                        case 6.5f:
                            cost = 600;
                            break;
                        case 7:
                            cost = 900;
                            break;
                        default:
                            cost = 100;
                            break;
                    }

                    if (world.Player.PlayerMoney >= cost)
                    {
                        world.Player.Gun.FireRatePerSecond = newFireRate;
                        world.Player.PlayerMoney -= cost;
                    }

                }
            };

            this.FireRate.OnHover += (object sender, EventArgs e) =>
            {
                if (this._UIScene.Scene is GameWorld world)
                {
                    float currentFireRate = world.Player.Gun.FireRatePerSecond;

                    if (currentFireRate == this._maxFireRate)
                    {
                        this.DescHoverCardText.Text = $"{currentFireRate} BPS";
                        this.CostHoverCardText.Text = "Max Level";
                        return;
                    }

                    float newFireRate = currentFireRate + this._fireRateIncrementAmount;

                    int cost = 0;

                    switch (newFireRate)
                    {
                        case 5:
                            cost = 150;
                            break;
                        case 5.5f:
                            cost = 250;
                            break;
                        case 6:
                            cost = 400;
                            break;
                        case 6.5f:
                            cost = 600;
                            break;
                        case 7:
                            cost = 900;
                            break;
                        default:
                            cost = 100;
                            break;
                    }

                    this.CostHoverCardText.Text = $"{cost} Coins";

                    this.CostHoverCardText.Colour = (world.Player.PlayerMoney < cost) ? Color.Red : Color.White;
                
                    this.DescHoverCardText.Text = $"{currentFireRate} -> {newFireRate} BPS";

                }
            };

            this.HealthCard.OnLeftClick += (object sender, EventArgs e) =>
            {
                if (this._UIScene.Scene is GameWorld world)
                {
                    int currentMaxHealth = world.Player.PlayerHealth.MaxHealth;

                    if(currentMaxHealth == this._maxHealth)
                        return;

                    int newMaxHealth = currentMaxHealth + this._maxHealthIncrementAmount;

                    int cost = 0;

                    switch (newMaxHealth)
                    {
                        case 125:
                            cost = 150;
                            break;
                        case 150:
                            cost = 250;
                            break;
                        case 175:
                            cost = 400;
                            break;
                        case 200:
                            cost = 600;
                            break;
                        case 225:
                            cost = 900;
                            break;
                        default:
                            cost = 100;
                            break;
                    }

                    if (world.Player.PlayerMoney >= cost)
                    {
                        world.Player.PlayerHealth.MaxHealth = newMaxHealth;
                        world.Player.PlayerMoney -= cost;
                    }

                }
            };

            this.HealthCard.OnHover += (object sender, EventArgs e) =>
            {
                if (this._UIScene.Scene is GameWorld world)
                {
                    int currentMaxHealth = world.Player.PlayerHealth.MaxHealth;

                    if (currentMaxHealth == this._maxHealth)
                    {
                        this.DescHoverCardText.Text = $"{currentMaxHealth} Max";
                        this.CostHoverCardText.Text = "Max Level";

                        return;
                    }
                        
                    int newMaxHealth = currentMaxHealth + this._maxHealthIncrementAmount;

                    int cost = 0;

                    switch (newMaxHealth)
                    {
                        case 125:
                            cost = 150;
                            break;
                        case 150:
                            cost = 250;
                            break;
                        case 175:
                            cost = 400;
                            break;
                        case 200:
                            cost = 600;
                            break;
                        case 225:
                            cost = 900;
                            break;
                        default:
                            cost = 100;
                            break;
                    }

                    this.CostHoverCardText.Text = $"{cost} Coins";

                    this.CostHoverCardText.Colour = (world.Player.PlayerMoney < cost) ? Color.Red : Color.White;
                
                    this.DescHoverCardText.Text = $"{currentMaxHealth} -> {newMaxHealth} Max";

                }
            };

            this.CostHoverCard = new UIAlignContainer()
            {
                TexturePath = "UI/MedUIBox",
                Name = "CostHoverCard",
                Width = 1200,
                Height = 200,
                HorizontalAlign = HorizontalAlign.Center,
                VerticalAlign = VerticalAlign.Center,
                IsVisable = false
            };

            this.CostHoverCardText = new UIText()
            {
                LocalPosition = Vector2.Zero,
                Colour = Color.White,
                Width = 100,
                Height = 50,
                FontSize = 64,
                Name = "CostHoverCard" + "Text",
                FontPath = "GameFont1",
                Text = "50 Coins",
                Parent = this.CostHoverCard
            };


            this._UIScene.AddUIProp(this.CostHoverCard);
            this._UIScene.AddUIProp(this.CostHoverCardText);

            this.DescHoverCard = new UIAlignContainer()
            {
                TexturePath = "UI/MedUIBox",
                Name = "DescHoverCard",
                Width = 1200,
                Height = 200,
                HorizontalAlign = HorizontalAlign.Center,
                VerticalAlign = VerticalAlign.Center,
                IsVisable = false,
                AnchorPoint = UIPropAnchorPoint.TopRight
            };

            this.DescHoverCardText = new UIText()
            {
                LocalPosition = Vector2.Zero,
                Colour = Color.White,
                Width = 100,
                Height = 50,
                FontSize = 64,
                Name = "DescHoverCard" + "Text",
                FontPath = "GameFont1",
                Text = "3 -> 4 BPS",
                Parent = this.DescHoverCard
            };


            this._UIScene.AddUIProp(this.DescHoverCard);
            this._UIScene.AddUIProp(this.DescHoverCardText);
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

                    this.CostHoverCard.IsVisable = true;

                    this.CostHoverCard.LocalPosition.Y = card.LocalPosition.Y + DescHoverCard.Height / 2;

                    this.DescHoverCard.IsVisable = true;

                    this.DescHoverCard.LocalPosition.Y = card.LocalPosition.Y + DescHoverCard.Height / 2;
                };
                card.OnHoverEnter += (object sender, EventArgs e) =>
                {
                    card.soundHolder.PlaySound("Hover");
                };
                card.OnHoverExit += (object sender, EventArgs e) =>
                {
                    card.OverlayColour = Color.White;

                    this.DescHoverCard.IsVisable = false;
                    this.CostHoverCard.IsVisable = false;
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