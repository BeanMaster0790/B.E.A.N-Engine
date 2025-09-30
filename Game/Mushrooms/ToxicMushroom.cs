using Bean;
using Bean.Debug;
using Bean.Graphics;
using Bean.Graphics.Lighting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DemoGame
{
    public class ToxicMushroom : Mushroom
    {
        [DebugServerVariable]
        public int DamageRange = 96;

        [DebugServerVariable]
        private int _numberOfRings = 5;

        private Timer attackTimer;

        [DebugServerVariable]
        private float _attackDelay = 0.75f;

        private float[] _rotations;

        private FlickeringLight _light;

        public override void Start()
        {
            base.Start();

            this.Target = ((GameWorld)this.Scene).Player;

            this.AnimationManager.ChangeTexture("Mushroom/GreenMushroom");

            this.AnimationManager.Play("WakingUp");

            this._light = new FlickeringLight(1f, DamageRange, DamageRange - 5, 5, 0.5f, Color.Green);

            this.AddAddon(this._light);

            this.SetTurretPoint();

            this._rotations = new float[this._numberOfRings];

            this.attackTimer = new Timer();

            this.AddAddon(attackTimer);

            this.attackTimer.StartTimer(this._attackDelay);

            this._minCoins = 8;
            this._maxCoins = 11;
        }

        public override void Update()
        {
            base.Update();

            if (Vector2.Distance(this.Position, this.Target.Position) < this.DamageRange)
                Attack();
        }

        protected override void Attack()
        {
            base.Attack();

            if (this.attackTimer.IsFinished && this._canAttack)
            {
                ((GameWorld)this.Scene).Player.PlayerHealth.RemoveHealth(this.Damage);

                this.attackTimer.StartTimer(this._attackDelay);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (this._light != null)
            {
                this._light.IsVisable = this._hasWokenUp;    
            }

            if (!this._hasWokenUp)
                return;

            int interval = this.DamageRange / this._numberOfRings;

            for (int i = 0; i < this._numberOfRings; i++)
            {
                if (i == 0)
                    continue;

                int radius = this.DamageRange - (interval * i);

                if ((i + 1) % 2 == 0)
                {
                    this._rotations[i] += 0.1f * Time.Instance.TargetMultiplier;
                }
                else
                {
                    this._rotations[i] -= 0.1f * Time.Instance.TargetMultiplier;
                }

                if (this._rotations[i] <= -360 || this._rotations[i] >= 360)
                {
                    this._rotations[i] = 0;
                }

                spriteBatch.Draw(Shapes.Circle(radius, Color.Green), this.Position - this.Scene.Camera.Position, null, Color.White, this._rotations[i], new Vector2(radius, radius), 1f, SpriteEffects.None, 0.1f);
            }
        }
    }
}