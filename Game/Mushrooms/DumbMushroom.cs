using Bean;
using Bean.Debug;
using Bean.Graphics.Lighting;
using Microsoft.Xna.Framework;

namespace DemoGame
{
    public class DumbMushroom : Mushroom
    {
        private bool _hasExploded; 

        public override void Start()
        {
            base.Start();

            this.AddAddon(new Light(0.5f, 12, Color.Red));

            this.Damage = 7;
        }

        public override void Update()
        {
            base.Update();

            if (Vector2.Distance(this.Position, this.Target.Position) < 64)
            {
                this.Attack();
            }
        }

        protected override void Attack()
        {
            base.Attack();

            if (this._hasExploded)
                return;

            int spores = this.Damage;

            for (int i = 0; i < spores; i++)
            {
                AttackSpore spore = new AttackSpore(this.Target, 1)
                {
                    Position = this.Position
                };

                this.Scene.AddToScene(spore);
            }

            this.Destroy();

            this._hasExploded = true;
        }
    }
}