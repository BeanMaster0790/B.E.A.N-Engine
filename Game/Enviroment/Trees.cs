using Bean;
using Bean.Graphics;
using Bean.Graphics.Lighting;
using Bean.PhysicsSystem;
using Bean.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DemoGame
{
    class Tree : Sprite
    {
        public override void Start()
        {
            base.Start();

            this.Tag = "NoSpawn";

            this.TexturePath = (Random.RandomInt(0, 5) != 1) ? "Trees/BigTree" : "Trees/SmallTree";

            this.AddAddon(new YSorter()
            {   
                YOrigin = (this.TexturePath == "Trees/SmallTree") ? 15 : 0
            });

            if (this.TexturePath == "Trees/BigTree")
            {
                this.AddAddon(new Collider()
                {
                    Width = (int)(30 * this.Scale),
                    Height = (int)(15 * this.Scale),
                    PositionOffset = new Vector2(8, 40)
                });
            }
            else
            {
                this.AddAddon(new Collider()
                {
                    Width = (int)(8 * this.Scale),
                    Height = (int)(3 * this.Scale),
                    PositionOffset = new Vector2(3, 28)
                });
            }

        }

        public override void Update()
        {
            base.Update();      
        }
    }
}