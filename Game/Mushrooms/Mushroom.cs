using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Bean;
using Bean.Debug;
using Bean.Graphics;
using Bean.Graphics.Animations;
using Bean.PhysicsSystem;
using Bean.Sounds;
using DemoGame;
using Microsoft.Xna.Framework;
using Random = Bean.Random;

namespace DemoGame
{
    public class Mushroom : Sprite
    {
        protected PhysicsObject _physicsObject;

        protected ContextSteering _contextSteering;

        private SoundHolder _soundHolder;

        public Sprite Target;

        [DebugServerVariable]
        protected bool _hasWokenUp;

        [DebugServerVariable]
        protected float _hopSpeed = 0.1f;

        [DebugServerVariable]
        protected float _hopDistance = 0.15f;

        [DebugServerVariable]
        protected Vector2 _moveVector;

        [DebugServerVariable]
        protected bool _isMoving;

        [DebugServerVariable]
        protected bool _canMove = true;

        [DebugServerVariable]
        protected bool _canAttack = true;

        [DebugServerVariable]
        public int Damage = 1;

        [DebugServerVariable]
        protected int _maxCoins = 5;

        [DebugServerVariable]
        protected int _minCoins = 3;

        protected Color _sporeColour;

        public Health Health;

        public EventHandler OnDeath;

        public override void Start()
        {
            base.Start();

            this.Tag = "Mushroom";

            this.AddAddon(new AnimationManager("Mushroom/RedMushroom"));

            this._physicsObject = new PhysicsObject();

            this.AddAddon(this._physicsObject);

            this.Target = this.Scene.GetPropWithName<Player>("Player");

            this._contextSteering = new ContextSteering(32, 16, new string[] { "NoSpawn", "Wall" }, new string[] { "Mushroom" }, this.Target) { IsActive = false };

            this.AddAddon(this._contextSteering);

            this.AddAddon(new Collider()
            {
                //Width = (int)(8 * this.Scale),
                //Height = (int)(6 * this.Scale),
                Width = 16,
                Height = 16,
                //PositionOffset = new Vector2(4 * this.Scale, 10 * this.Scale)
            });

            this.AddAddon(new YSorter());

            this._soundHolder = new SoundHolder();

            this.AddAddon(this._soundHolder);

            this._soundHolder.AddSound("Pop", "Sounds/Pop", true);

            this.Health = new Health(3);

            this.Health.OnDeath += (object sender, EventArgs e) =>
            {
                int coins = Random.RandomInt(this._minCoins, this._maxCoins);

                for (int i = 0; i < coins; i++)
                {
                    Coin coin = new Coin(((GameWorld)this.Scene).Player)
                    {
                        Position = this.Position
                    };

                    this.Scene.AddToScene(coin);
                }

                this._soundHolder.PlaySound("Pop", this.Position);

                this.OnDeath?.Invoke(this, null);

                this.Destroy();
            };

            this.AddAddon(this.Health);

            this.AnimationManager.Play("WakingUp");

            this.AnimationManager.FrameEvent += (object sender, FrameEventArgs e) =>
            {
                if (e.AnimationName == "WakingUp" && e.CurrentFrame == this.AnimationManager.Animations[e.AnimationName].FrameCount)
                {
                    this._hasWokenUp = true;

                    this.AnimationManager.Animations["Walking"].FrameSpeed = this._hopSpeed;
                    this.AnimationManager.Play("Walking");
                }

                if (e.AnimationName == "Walking" && e.CurrentFrame >= 7 && e.CurrentFrame <= 12)
                {
                    this._isMoving = true;
                    this._moveVector = this._contextSteering.MoveVector;

                }
                else
                {
                    this._isMoving = false;
                    this._physicsObject.Velocity = Vector2.Zero;
                }

                if (e.AnimationName == "Walking" && e.CurrentFrame == 7)
                {
                    this._contextSteering.Update();
                }
            };

            this._sporeColour = Color.Red;
        }

        public virtual void SetTurretPoint()
        {
            Sprite steeringSprite = new Sprite() { Position = this.Position };

            this._contextSteering.Target = steeringSprite;
            this.Position = steeringSprite.Position;

            this._physicsObject.Destroy();
        }

        protected virtual void Attack()
        {

        }

        public override void Update()
        {
            base.Update();

            if (!this._hasWokenUp)
                return;

            if (this._canMove)
            {
                if (this._isMoving)
                    this._physicsObject.Velocity = this._moveVector * this._hopDistance;
            }
            else
            {
                this._moveVector = Vector2.Zero;
                this._contextSteering.MoveVector = Vector2.Zero;
            }
        }
    }
}