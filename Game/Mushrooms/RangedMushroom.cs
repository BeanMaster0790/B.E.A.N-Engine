using Bean;
using Bean.Debug;
using Bean.Graphics.Lighting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DemoGame
{
    public class RangedMushroom : Mushroom
    {
        protected Gun _gun;

        [DebugServerVariable]
        protected int _attackRange = 100;

        [DebugServerVariable]
        protected int _distanceRange = 30;

        [DebugServerVariable]
        protected RangedMushroomState _state;

        public override void Start()
        {
            base.Start();

            this._gun = new Gun()
            {
                MaxAmmo = 999,
                PeircingChance = 0,
                FireRatePerSecond = 1,
                Target = this.Scene.GetPropWithName<Player>("Player"),
                Parent = this,
            };

            this._attackRange = 275;

            this._gun.FireRatePerSecond = 0.5f;

            this._gun.BulletSpeed = 0.2f;

            this.AddAddon(new Light(0.5f, 12, Color.Brown));

            this.Target = this.Scene.GetPropWithName<Player>("Player");

            this.SetTurretPoint();

            this.AnimationManager.ChangeTexture("Mushroom/BrownMushroom");

            this.AnimationManager.Play("WakingUp");

            this.Scene.AddToScene(this._gun);

            this._minCoins = 7;
            this._maxCoins = 10;

            this._gun.Damage = 3;

            this._sporeColour = Color.Brown;
        }


        public override void Update()
        {
            base.Update();

            if (!this._hasWokenUp)
                return;

            DebugManager.Instance.DrawCircle(this.Position, this._attackRange, Color.OrangeRed);
            DebugManager.Instance.DrawCircle(this.Position, this._distanceRange, Color.Red);

            if (Vector2.Distance(this.Position, this.Target.Position) < this._distanceRange)
            {
                this._state = RangedMushroomState.Distance;

                if(this._isMoving)
                    this._physicsObject.Velocity = -this._contextSteering.MoveVector * this._hopDistance; 
            }
            else if (Vector2.Distance(this.Position, this.Target.Position) < this._attackRange)
            {
                this._state = RangedMushroomState.Attack;

                if(this._canAttack)
                    this.Attack();
            }
            else
            {
                this._state = RangedMushroomState.Hunt;
            }
        }

        protected override void Attack()
        {
            this._gun.Fire(this._sporeColour, "Guns/Spore", 1);
        }

    }

    public enum RangedMushroomState
    {
        Hunt,
        Attack,
        Distance
    }
}