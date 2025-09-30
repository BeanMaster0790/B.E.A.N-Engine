using System;
using Bean;
using Bean.Debug;
using Bean.Graphics;
using Bean.Graphics.Lighting;
using Bean.PhysicsSystem;
using Bean.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DemoGame
{
    public class Gun : Sprite
    {
        [DebugServerVariable]
        public float FireRatePerSecond = 4;

        [DebugServerVariable]
        public int MaxAmmo = 10;

        [DebugServerVariable]
        public int CurrentAmmo { private set; get; }

        [DebugServerVariable]
        public int PeircingChance = 0;

        [DebugServerVariable]
        public float BulletSpeed = 2;

        public Sprite Parent;

        public Sprite Target;

        private Timer _fireRateTimer;

        [DebugServerVariable]
        public int Damage = 1;

        public override void Start()
        {
            base.Start();

            this.PropID = "Gun";

            this._fireRateTimer = new Timer();

            this._fireRateTimer.StartTimer(1 / FireRatePerSecond);

            this.AddAddon(_fireRateTimer);

            this.CurrentAmmo = this.MaxAmmo;
        }

        public override void Update()
        {
            base.Update();

            if (Parent.ToRemove)
                this.Destroy();

            this.Position = Parent.Position;
            this.Layer = Parent.Layer + 0.001f; 

            Vector2 diffrence = ((this.Target == null) ? this.Scene.Camera.ScreenToWorld(InputManager.Instance.MousePosition()) : this.Target.Position) - this.Position;

            diffrence.Normalize();

            float rotation = MathF.Atan2(diffrence.Y, diffrence.X);

            this.Rotation = MathHelper.ToDegrees(rotation);

            Vector2 offsetDirection = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));

            this.Position = Parent.Position + offsetDirection * 10;

            if (this.Rotation < -90 || this.Rotation > 90)
            {
                this.spriteEffect = SpriteEffects.FlipVertically;
                Parent.spriteEffect = SpriteEffects.FlipHorizontally;
            }
            else
            {
                this.spriteEffect = SpriteEffects.None;
                Parent.spriteEffect = SpriteEffects.None;
            }
        }

        public void Fire(Color bulletColour, string texturePath, float scale)
        {
            if (this._fireRateTimer == null || this.Parent == null)
                return;

            if (this._fireRateTimer.IsFinished && this.CurrentAmmo > 0)
                {
                    Vector2 diffrence = ((this.Target == null) ? this.Scene.Camera.ScreenToWorld(InputManager.Instance.MousePosition()) : this.Target.Position) - this.Position;

                    diffrence.Normalize();

                    Bullet bullet = new Bullet(this.Damage, this.PeircingChance, this.Target, this.BulletSpeed)
                    {
                        Texture = FileManager.LoadFromFile<Texture2D>(texturePath),
                        Scale = scale,
                        Rotation = this.Rotation,
                        Position = new Vector2(this.Position.X, this.Position.Y) + diffrence * ((this.Texture == null) ? this.Parent.GetSpriteRectangle().Width : this.Texture.Width / 2),
                        Colour = bulletColour
                    };

                    this.Scene.AddToScene(bullet);

                    this._fireRateTimer.StartTimer(1 / FireRatePerSecond);
                }
        }
    }
}