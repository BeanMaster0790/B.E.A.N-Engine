using Microsoft.Xna.Framework;
using Bean;

namespace DemoGame
{
    public class AttackSpore : Coin
    {
        private int _damage;

        public AttackSpore(WorldProp target, int damage) : base(target)
        {
            this._damage = damage;
        }

        public override void Start()
        {
            base.Start();

            this.TexturePath = "Guns/Spore";

            this.Colour = Color.Red;

            this.Scale = 1f;

            DestoryAfterSeconds(3);
        }

        public override void Activate()
        {
            Player target = (Player)this.Target;

            target.PlayerHealth.RemoveHealth(this._damage);

            this.Destroy();
        }
    }
}