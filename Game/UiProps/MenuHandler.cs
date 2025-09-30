using System;
using System.Collections.Generic;
using Bean;
using Bean.Debug;
using Bean.Graphics;
using Bean.Scenes;
using Bean.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Random = Bean.Random;

namespace DemoGame
{
    public class MenuHandler
    {
        public UIScene UIScene;

        public Scene Scene;

        public MainMenuList MainMenuList;

        public SettingsMenu SettingsMenu;

        public GraphicSettingsMenu GraphicSettings;

        public FullScreenMenu ResolutionSettings;

        public FPSMenu FPSMenu;

        public ScalingSettingsMenu ScalingSettings;

        public UpgradeMenu UpgradeMenu;

        public bool StartGame;

        public Dictionary<string, Menu> Menus = new Dictionary<string, Menu>();

        private bool _menuOpenedThisFrame;

        public bool IsOpen;

        public string CurrentMenu;

        public MenuHandler(Scene scene, UIScene uIScene)
        {
            this.Scene = scene;
            this.UIScene = uIScene;
        }


        public void OpenMenu(string menu)
        {
            if (this._menuOpenedThisFrame)
                return;

            this._menuOpenedThisFrame = true;

            foreach (KeyValuePair<string, Menu> item in this.Menus)
            {
                item.Value.IsActive = false;
                item.Value.IsVisable = false;

                item.Value._scroll = 0;
            }

            this.CurrentMenu = menu;

            if (string.IsNullOrEmpty(menu))
            {
                this.IsOpen = false;
                return;
            }

            this.IsOpen = true;

            this.Menus[menu].IsActive = true;
            this.Menus[menu].IsVisable = true;
        }

        public void LateUpdate()
        {
            this._menuOpenedThisFrame = false;
        }


        public void LoadScene()
        {
            Game game = (Game)Engine.Instance;

            this.MainMenuList = new MainMenuList(this.Scene, "Main")
            {
                AnchorPoint = Bean.UI.UIPropAnchorPoint.Center,
                HorizontalAlign = Bean.UI.HorizontalAlign.Center,
                VerticalAlign = Bean.UI.VerticalAlign.Center,
                Width = 600,
                Height = 1000,
                Name = "Main"
            };

            this.UIScene.AddUIProp(this.MainMenuList);

            this.SettingsMenu = new SettingsMenu(this.Scene, "Settings")
            {
                AnchorPoint = Bean.UI.UIPropAnchorPoint.Center,
                HorizontalAlign = Bean.UI.HorizontalAlign.Center,
                VerticalAlign = Bean.UI.VerticalAlign.Center,
                Width = 600,
                Height = 1000,
                IsVisable = false,
                Name = "Settings"
            };

            this.UIScene.AddUIProp(this.SettingsMenu);

            this.GraphicSettings = new GraphicSettingsMenu(this.Scene, "Video")
            {
                AnchorPoint = Bean.UI.UIPropAnchorPoint.Center,
                HorizontalAlign = Bean.UI.HorizontalAlign.Center,
                VerticalAlign = Bean.UI.VerticalAlign.Center,
                isScrollable = true,
                Width = 600,
                Height = 1000,
                IsVisable = false,
                Name = "Video"
            };

            this.UIScene.AddUIProp(this.GraphicSettings);

            this.ResolutionSettings = new FullScreenMenu(this.Scene, "Resolution")
            {
                AnchorPoint = Bean.UI.UIPropAnchorPoint.Center,
                HorizontalAlign = Bean.UI.HorizontalAlign.Center,
                VerticalAlign = Bean.UI.VerticalAlign.Center,
                isScrollable = true,
                Width = 600,
                Height = 1000,
                IsVisable = false,
                Name = "Resolution"
            };

            this.UIScene.AddUIProp(this.ResolutionSettings);

            this.FPSMenu = new FPSMenu(this.Scene, "FPS")
            {
                AnchorPoint = Bean.UI.UIPropAnchorPoint.Center,
                HorizontalAlign = Bean.UI.HorizontalAlign.Center,
                VerticalAlign = Bean.UI.VerticalAlign.Center,
                isScrollable = true,
                Width = 600,
                Height = 1000,
                IsVisable = false,
                Name = "FPS"
            };

            this.UIScene.AddUIProp(this.FPSMenu);

            this.ScalingSettings = new ScalingSettingsMenu(this.Scene, "Scale")
            {
                AnchorPoint = Bean.UI.UIPropAnchorPoint.Center,
                HorizontalAlign = Bean.UI.HorizontalAlign.Center,
                VerticalAlign = Bean.UI.VerticalAlign.Center,
                isScrollable = true,
                Width = 600,
                Height = 1000,
                IsVisable = false,
                Name = "Scale"
            };

            this.UIScene.AddUIProp(this.ScalingSettings);

            this.UpgradeMenu = new UpgradeMenu(this.Scene, "Upgrades")
            {
                AnchorPoint = Bean.UI.UIPropAnchorPoint.Center,
                HorizontalAlign = Bean.UI.HorizontalAlign.Center,
                VerticalAlign = Bean.UI.VerticalAlign.Center,
                isScrollable = true,
                Width = 600,
                Height = 1000,
                IsVisable = false,
                Name = "Scale"
            };

            this.UIScene.AddUIProp(this.UpgradeMenu);
        }
    }
}