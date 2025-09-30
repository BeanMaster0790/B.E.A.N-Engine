using System;
using System.Collections.Generic;
using Bean;
using Bean.Debug;
using Bean.Graphics;
using Bean.Graphics.Animations;
using Bean.Graphics.Lighting;
using Bean.PhysicsSystem;
using Bean.Player;
using Bean.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DemoGame
{

    public class Player : Sprite
    {
        private PhysicsObject _physicsObject;

        [DebugServerVariable]
        private float _maxSpeed = 0.5f;

        public Gun Gun;

        public Health PlayerHealth;

        public SoundHolder SoundHolder;

        private Texture2D _vin;

        [DebugServerVariable]
        public int PlayerMoney = 10000;

        private bool _lowHealth;

        private Timer SlowTimeTimer;

        private Timer SlowTimeCooldown;

        private Timer SlowTimeNoiseDelay;

        [DebugServerVariable]
        public bool SlowTime;

        [DebugServerVariable]
        public float SlowTimeDuration = 6;

        [DebugServerVariable]
        public float SlowTimeCooldownDuration = 10;

        private List<SlowStampData> _slowStamps = new List<SlowStampData>();

        [DebugServerVariable]
        private float _stampFrequency = 0.08f;

        [DebugServerVariable]
        private int _maxStamps = 4;

        private float _stampTimer = 0;

        [DebugServerVariable]
        private Color _slowDownColour = new Color(105, 35, 35);

        public EventHandler StartSlowTime;
        public EventHandler EndSlowTime;

        public override void Start()
        {
            base.Start();

            this.PropID = "Player";

            this.Scale = 1f;

            this.AddAddon(new AnimationManager("Bean/Bean"));

            this._physicsObject = new PhysicsObject();

            this.AddAddon(this._physicsObject);

            this.AnimationManager.Play("Idle");

            this.PlayerHealth = new Health(100, finalStand: true);

            this.AddAddon(this.PlayerHealth);

            this.AddAddon(new YSorter());

            Collider collider = new Collider()
            {
                Width = (int)(8 * this.Scale),
                Height = (int)(6 * this.Scale),
                PositionOffset = new Vector2(4 * this.Scale, 10 * this.Scale)
            };

            this.AddAddon(collider);

            this.AddAddon(new FlickeringLight(0.75f, 24, 20, 3, 0.5f, Color.Orange));

            this.Gun = new Gun()
            {
                Texture = FileManager.LoadFromFile<Texture2D>("Guns/Revolver"),
                Scale = 0.75f,
                Parent = this,
            };

            this.Scene.AddToScene(this.Gun);

            this.SoundHolder = new SoundHolder();

            this.AddAddon(this.SoundHolder);

            this.SoundHolder.AddSound("Coin1", "Sounds/Coin1");
            this.SoundHolder.AddSound("Coin2", "Sounds/Coin2");
            this.SoundHolder.AddSound("Coin3", "Sounds/Coin3");
            this.SoundHolder.AddSound("LowHealth", "Sounds/Heartbeat", isLooped: true);
            this.SoundHolder.AddSound("SlowDown", "Sounds/SlowDown");
            this.SoundHolder.AddSound("SlowTime", "Sounds/TimeSlow", isLooped: true);
            this.SoundHolder.AddSound("SpeedUp", "Sounds/SpeedUp");

            this.SlowTimeTimer = new Timer();

            this.AddAddon(this.SlowTimeTimer);

            this.SlowTimeTimer.StartTimer(0);

            this.SlowTimeCooldown = new Timer();

            this.AddAddon(this.SlowTimeCooldown);

            this.SlowTimeCooldown.StartTimer(0);

            this.SlowTimeNoiseDelay = new Timer();

            this.AddAddon(this.SlowTimeNoiseDelay);

            this.SlowTimeNoiseDelay.StartTimer(0);

            this._vin = FileManager.LoadFromFile<Texture2D>("LowHealth", true);
        }

        public override void Update()
        {
            base.Update();

            ((GameWorld)this.Scene).PlayerHealthTextSlider.FillValue = this.PlayerHealth.CurrentHealth;
            ((GameWorld)this.Scene).PlayerHealthTextSlider.MaxFill = this.PlayerHealth.MaxHealth;
            ((GameWorld)this.Scene).PlayerMoneyText.Text = this.PlayerMoney.ToString();

            if (InputManager.Instance.WasLeftButtonPressed())
            {
                this.Gun.Fire(Color.White, "Guns/Bullet", 2);
            }

            if (InputManager.Instance.WasKeyPressed(Keys.Space) && this.SlowTimeCooldown.IsFinished)
            {
                this.SlowTimeTimer.StartTimer(this.SlowTimeDuration);

                this.SoundHolder.PlaySound("SlowDown");

                this.SlowTimeCooldown.StartTimer(1000);

                this.StartSlowTime?.Invoke(this, null);

                this.SlowTimeNoiseDelay.StartTimer(2);
            }

            if (this.SlowTimeTimer.IsActive)
            {
                this.SlowTime = true;

                if (this.SlowTimeNoiseDelay.IsFinished)
                {
                    this.SoundHolder.PlaySound("SlowTime");

                    this.SlowTimeNoiseDelay.StartTimer(1000);    
                }
            }

            else if (!this.SlowTimeTimer.IsActive && this.SlowTime)
            {
                this.SlowTime = false;

                this.SoundHolder.StopSound("SlowTime", true);

                this.SoundHolder.PlaySound("SpeedUp");

                this.SlowTimeCooldown.StartTimer(this.SlowTimeCooldownDuration);

                this.EndSlowTime?.Invoke(this, null);
            }


            Vector2 moveVector = new Vector2();

            if (InputManager.Instance.IsKeyHeld(Keys.A))
                moveVector.X += -1;
            if (InputManager.Instance.IsKeyHeld(Keys.D))
                moveVector.X += 1;

            if (InputManager.Instance.IsKeyHeld(Keys.W))
                moveVector.Y -= 1;
            if (InputManager.Instance.IsKeyHeld(Keys.S))
                moveVector.Y += 1;

            if (moveVector != Vector2.Zero)
                moveVector.Normalize();

            this._physicsObject.Velocity = moveVector * this._maxSpeed;


            if (moveVector.X != 0)
            {
                this.AnimationManager.Play("WalkSide");
            }
            else if (moveVector.Y > 0)
                this.AnimationManager.Play("WalkDown");
            else if (moveVector.Y < 0)
                this.AnimationManager.Play("WalkUp");
            else
                this.AnimationManager.Play("Idle");

            if (this.PlayerHealth.CurrentHealth < 25)
            {

            }
        }

        public override void LateUpdate()
        {
            base.LateUpdate();

            if(this.SlowTime)
                this.Scene.LightingManager.GlobalColour = this._slowDownColour;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (this.PlayerHealth == null)
                return;

            if (this.PlayerHealth.CurrentHealth < 25)
            {
                Vector2 textureSize = new Vector2(this._vin.Width, this._vin.Height);

                Vector2 screenSize = new Vector2(this.Scene.Camera.GetWidth(), this.Scene.Camera.GetHeight());

                Vector2 scaleFactor = screenSize / textureSize;

                spriteBatch.Draw(this._vin, Vector2.Zero, null, Color.White, 0, new Vector2(textureSize.X / 2, textureSize.Y / 2), scaleFactor * 1.05f, SpriteEffects.None, 1);

                if (this._lowHealth == false)
                    this.SoundHolder.PlaySound("LowHealth");

                this._lowHealth = true;
            }
            else
            {
                if (_lowHealth == true)
                    this.SoundHolder.StopSound("LowHealth", true);

                this._lowHealth = false;
            }

            if (this.SlowTime)
            {
                this._stampTimer += Time.Instance.DeltaTime;

                if (this._stampTimer > this._stampFrequency)
                {
                    this._slowStamps.Add(new SlowStampData(this.Position, this.AnimationManager.CurrentAnimation.GetTextureRectangle()));

                    if (this._slowStamps.Count > this._maxStamps)
                        this._slowStamps.RemoveAt(0);

                    this._stampTimer = 0;
                }

                foreach (SlowStampData data in this._slowStamps)
                {
                    spriteBatch.Draw(this.AnimationManager.Texture, data.Position - base.Scene.Camera.Position, data.TextureRectangle, this.Colour * 0.4f, MathHelper.ToRadians(this.Rotation), this.GetOrigin(), this.Scale, this.spriteEffect, this.Layer);
                }
            }
            else if (this._slowStamps.Count > 0)
            {
                this._stampTimer += Time.Instance.DeltaTime;

                if (this._stampTimer > this._stampFrequency)
                {
                    this._slowStamps.RemoveAt(0);

                    this._stampTimer = 0;
                }

                foreach (SlowStampData data in this._slowStamps)
                {
                    spriteBatch.Draw(this.AnimationManager.Texture, data.Position - base.Scene.Camera.Position, data.TextureRectangle, this.Colour * 0.2f, MathHelper.ToRadians(this.Rotation), this.GetOrigin(), this.Scale, this.spriteEffect, this.Layer);
                }
            }
        }
    }

}