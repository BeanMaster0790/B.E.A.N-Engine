using System;
using Bean;
using Bean.PhysicsSystem;
using Microsoft.Xna.Framework;
using Random = Bean.Random;

namespace DemoGame
{
    public class Coin : Sprite
    {
        private PhysicsObject _physicsObject;

        private Timer _homingTimer;

        public WorldProp Target;

        protected int _pickupRange = 10;

        public Coin(WorldProp target)
        {
            this.Target = target;
        }

        public override void Start()
        {
            base.Start();

            this._physicsObject = new PhysicsObject();

            this.AddAddon(this._physicsObject);

            this._homingTimer = new Timer();

            this.AddAddon(this._homingTimer);

            this._homingTimer.StartTimer(0.2f);

            this.TexturePath = "UI/CoinIcon";

            this.Scale = 0.25f;

            this.Layer = 1;

            float angle = Random.RandomFloat(0, MathF.PI * 2);
            float speed = Random.RandomFloat(0.5f, 0.9f);
            this._physicsObject.Velocity = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * speed;
        }

        public override void Update()
        {
            base.Update();

            const float minSpeed = 0.5f;       // slowest speed (when close to player)
            const float maxSpeed = 2.0f;       // fastest speed (when far)
            const float speedScale = 0.5f;      // scales speed with distance
            const float steeringStrength = 0.05f; // how strongly it turns toward player
            const float swirlStrength = 0.04f; // side force to make it arc
            const float maxVelocity = 1.2f;  // hard clamp on velocity

            if (this._homingTimer.IsFinished)
            {
                Vector2 toPlayer = this.Target.Position - this.Position;
                float distance = toPlayer.Length();

                if (distance > this._pickupRange)
                {
                    // Direction to player
                    Vector2 dir = Vector2.Normalize(toPlayer);

                    // Speed grows with distance but is clamped
                    float targetSpeed = Math.Clamp(distance * speedScale, minSpeed, maxSpeed);
                    Vector2 desired = dir * targetSpeed;

                    // Steering force toward desired velocity
                    Vector2 steering = (desired - this._physicsObject.Velocity) * steeringStrength;

                    // Perpendicular swirl
                    Vector2 perp = new Vector2(-dir.Y, dir.X);
                    steering += ((Random.RandomInt(0, 1) == 1) ? perp : -perp) * swirlStrength;

                    // Apply steering
                    this._physicsObject.Velocity += steering;

                    // Clamp velocity magnitude
                    if (this._physicsObject.Velocity.Length() > maxVelocity)
                        this._physicsObject.Velocity = Vector2.Normalize(this._physicsObject.Velocity) * maxVelocity;
                }
                else
                {
                    this.Activate();
                }
            }

            if (this.Scene is GameWorld world)
            {
                if (world.Player.SlowTime && this is not AttackSpore)
                    this.LateUpdate();    
            }
        }

        public virtual void Activate()
        {
            ((GameWorld)this.Scene).Player.PlayerMoney++;
            ((GameWorld)this.Scene).Player.SoundHolder.PlaySound($"Coin{Random.RandomInt(1, 4)}");
            this.Destroy();
        }


    }
}