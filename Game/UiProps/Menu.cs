using System;
using Bean.Scenes;
using Bean.UI;

namespace DemoGame
{
    public class Menu : UIAlignContainer
    {
        public MenuHandler MenuHandler;
        public Menu(Scene scene, string key)
        {
            if(scene is MainMenu)
                this.MenuHandler = ((MainMenu)scene).MenuHandler;

            if(scene is GameWorld)
                this.MenuHandler = ((GameWorld)scene).MenuHandler;

            this.MenuHandler.Menus.Add(key, this);
        }
    }
}