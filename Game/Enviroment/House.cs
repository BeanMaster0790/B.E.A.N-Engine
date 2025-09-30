using Bean;
using Bean.Debug;
using Bean.Graphics;
using Bean.PhysicsSystem;
using Bean.UI;
using Microsoft.Xna.Framework;

namespace DemoGame
{
    public class House : Sprite
    {
        public override void Start()
        {
            base.Start();

            Collider collider = new Collider() { Width = 55, Height = 25, PositionOffset = new Vector2(3, 50) };

            this.AddAddon(collider);

            this.AddAddon(new YSorter());

        }
    }
}