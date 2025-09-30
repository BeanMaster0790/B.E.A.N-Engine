using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Bean;
using Bean.Debug;
using Bean.Graphics.Lighting;
using Bean.PhysicsSystem;
using Bean.Player;
using Microsoft.Xna.Framework;
using Random = Bean.Random;

namespace DemoGame
{
    public class DodgingMushroom : RangedMushroom
    {
        public override void Start()
        {
            base.Start();

            this._attackRange = 275;

            this._distanceRange = 85;

            this._gun.FireRatePerSecond = 1;

            this._gun.BulletSpeed = 0.2f;

            this.SetTurretPoint();

            this.AnimationManager.ChangeTexture("Mushroom/PurpleMushroom");

            this.AnimationManager.Play("WakingUp");

            this.AddAddon(new Light(0.5f, 12, Color.Purple));

            this.Target = this.Scene.GetPropWithName<Player>("Player");

            this._minCoins = 12;
            this._maxCoins = 17;

            this._sporeColour = Color.Purple;
        }

        public override void Update()
        {
            base.Update();

            if (this._state == RangedMushroomState.Distance || ((GameWorld)this.Scene).Player.SlowTime)
            {
                this._canMove = false;

                this.GetAddon<Collider>().IsSolid = true;

                this.Attack();

                return;
            }
            else if (!this.Scene.Camera.InCameraBounds(this.Position, this.GetSpriteRectangle().Width, this.GetSpriteRectangle().Height, new Vector2(this.GetSpriteRectangle().Width / 2, this.GetSpriteRectangle().Height / 2)))
            {
                this._canMove = true;

                this.GetAddon<Collider>().IsSolid = false;

                return;
            }
            else if (this._state == RangedMushroomState.Hunt)
            {
                this._canMove = true;
            }
            else
            {
                if (this._contextSteering.TargetReached)
                    this._canMove = false;

                else
                    this._canMove = true;
            }

            this._canAttack = true;

            this.GetAddon<Collider>().IsSolid = false;

            Vector2 diffrence = this.Scene.Camera.ScreenToWorld(InputManager.Instance.MousePosition()) - this.Target.Position;

            diffrence.Normalize();

            int distance = (int)Vector2.Distance(this.Position, this.Target.Position) + 20;

            RayHit rayHit = Raycasts.Instance.ShootRay(this.Target.Position, diffrence, distance, this.Target, new string[] {"VillageBounds", "Bullet"});

            if (rayHit != null && rayHit.Collider.Parent == this)
            {
                DebugManager.Instance.DrawLine(Target.Position, diffrence, distance, Color.Red, 1);
            }
            else
            {
                DebugManager.Instance.DrawLine(Target.Position, diffrence, distance, Color.Green, 1);
            }

            if (InputManager.Instance.WasLeftButtonPressed())
            {
                if (rayHit != null && rayHit.Collider.Parent == this)
                {
                    float randomX = (Random.RandomInt(0, 1) == 0) ? -3 : 3;

                    float randomY = (Random.RandomInt(0, 1) == 0) ? -3 : 3;

                    while (true)
                    {
                        Collider[] colliders = Raycasts.Instance.OverlapBoxAll(this.Position + new Vector2(randomX, randomY) * this._hopDistance * 15, 24);

                        foreach (Collider collider in colliders)
                        {
                            if (collider.Parent.Tag == "NoSpawn")
                            {
                                randomX = (Random.RandomInt(0, 1) == 0) ? -3 : 3;

                                randomY = (Random.RandomInt(0, 1) == 0) ? -3 : 3;

                                continue;
                            }
                        }

                        break;

                    }

                    this.Position += new Vector2(randomX, randomY) * this._hopDistance * 15;
                }
            }
        }
    }
}