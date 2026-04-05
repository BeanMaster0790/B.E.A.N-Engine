using Bean.Debug;
using Bean.JsonVariables;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Bean.PhysicsSystem {

    public class PhysicsObject : Addon
    {
        [DebugServerVariable]
        public Vector2 Velocity;

        private Collider _collider;

        private bool _bypassChecks;

        public PhysicsObject(string name,bool bypassMovmentOnlyChecks = false) :  base(name)
        {
            this._bypassChecks = bypassMovmentOnlyChecks;
        }

        public override void Start()
        {
            base.Start();

            this._collider = Parent.GetAddon<Collider>();

            if (this._collider != null)
            {
                this._collider.OnCollisionStay += this.OnCollide;
            }
        }

        public override void LateUpdate()
        {
            base.Update();

            this.Parent.PropTransform.Position += Velocity * Time.Instance.TargetMultiplier;

            if (this._collider != null && !this._bypassChecks)
                Physics.Instance.MovingColliders.Add(this._collider);
        }

        private void OnCollide(object sender, CollisionEventArgs e)
        {
            if (!e.Collider.IsSolid)
                return;

            if (e.Direction.X > 0 && this.Velocity.X < 0)
            {
                this.Velocity.X = 0;
            }
            else if (e.Direction.X < 0 && this.Velocity.X > 0)
            {
                this.Velocity.X = 0;
            }

            if (e.Direction.Y > 0 && this.Velocity.Y < 0)
            {
                this.Velocity.Y = 0;
            }
            else if (e.Direction.Y < 0 && this.Velocity.Y > 0)
            {
                this.Velocity.Y = 0;
            }
        }

        public override void Destroy()
        {
            base.Destroy();

            Collider collider = Parent.GetAddon<Collider>();

            if (collider != null)
            {
                collider.OnCollisionStay -= this.OnCollide;    
            }
        }
    } 
}