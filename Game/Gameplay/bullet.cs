using Bean;
using Bean.Debug;
using Bean.Graphics;
using Bean.Graphics.Lighting;
using Bean.PhysicsSystem;
using Bean.Player;
using Microsoft.Xna.Framework;

namespace DemoGame
{
    public class Bullet : Sprite
    {
        public int Damage;

        private int _pericingChance;

        private Sprite _target;

        private float _bulletSpeed;

        private bool _hasAtacked;

        public Bullet(int damage, int peircingChance, Sprite target, float bulletSpeed)
        {
            this.Damage = damage;

            this._pericingChance = peircingChance;

            this._target = target;

            this._bulletSpeed = bulletSpeed;
        }

        public override void Start()
        {
            base.Start();

            this.Name = "Bullet";

            this.Tag = "Bullet";

            this.AddAddon(new YSorter());

            this.AddAddon(new Collider()
            {
                Width = this.Texture.Width,
                Height = this.Texture.Height,
                IsSolid = false,
                IgnoreTags = new string[] { "Bullet" }
            });

            this.GetAddon<Collider>().OnCollisionEnter += Collide;

            this.AddAddon(new PhysicsObject());

            Vector2 diffrence = ((this._target == null) ? this.Scene.Camera.ScreenToWorld(InputManager.Instance.MousePosition()) : this._target.Position) - this.Position;

            diffrence.Normalize();

            this.GetAddon<PhysicsObject>().Velocity = diffrence * this._bulletSpeed;

            this.DestoryAfterSeconds(Random.RandomFloat(10, 15));
        }

        private void Collide(object sender, CollisionEventArgs collision)
        {

            if (collision.Collider.IsSolid && (collision.Collider.Parent == this._target || this._target == null))
            {
                if (this._hasAtacked)
                    return;

                Health health = collision.Collider.Parent.GetAddon<Health>();

                if (health != null)
                {
                    this._hasAtacked = true;
                    health.RemoveHealth(this.Damage);
                }

            }

            if (!collision.Collider.IsSolid)
                return;

            this.Destroy();
        }

        public override void Update()
        {
            base.Update();

            if (this.Scene is GameWorld world)
            {
                if (((GameWorld)this.Scene).IsGameSlow && this._target == null)
                    this.LateUpdate();    
            }
        }
    }
}