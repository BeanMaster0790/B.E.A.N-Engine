using System;
using Bean;
using Bean.Graphics;
using Bean.PhysicsSystem;
using Bean.Player;
using Microsoft.Xna.Framework;

namespace DemoGame
{
    public class CameraController : WorldProp 
    {
        public WorldProp Target;

        [DebugServerVariable]
        public float FollowSpeed = 0.05f;

        [DebugServerVariable]
        private int _heightWhenSlow = 195;

        [DebugServerVariable]
        private float _timeToZoom = 0.5f;

        private int _heightWhenNotSlow = 250;

        private float _currentTimePassed;

        private Camera _cam;

        private PhysicsObject _physicsObject;

        private bool _zoomIn;
        private bool _zoomOut;

        public override void Start()
        {
            base.Start();

            this.PropID = "CameraController";

            this._cam = this.Scene.Camera;

            this._cam.SetZ(this._cam.GetZFromHeight(this._heightWhenNotSlow));

            if (this.Target is Player player)
            {
                player.StartSlowTime += (object sender, EventArgs e) => { this._zoomIn = true; };
                player.EndSlowTime += (object sender, EventArgs e) => { this._zoomOut = true;  };
            }
        }

        public override void LateUpdate()
        {
            base.Update();

            if (this.Target == null)
                return;

            this._physicsObject = this.Target.GetAddon<PhysicsObject>();

            bool followMouse = false;

            if (this._physicsObject != null && this._physicsObject.Velocity == Vector2.Zero)
            {
                followMouse = true;
            }

            Vector2 difference = this.Scene.Camera.ScreenToWorld(InputManager.Instance.MousePosition()) - this.Position;
            difference.Normalize();

            // sway offsets
            float time = (float)Time.Instance.SecondsSinceStart; // total elapsed time
            float swayX = (float)Math.Sin(time * 1.5f) * 3f; // speed * amplitude
            float swayY = (float)Math.Cos(time * 1.5f) * 2f;

            Vector2 sway = Vector2.Zero;

            if (Target is Player player)
            {

                if (player.PlayerHealth != null && player.PlayerHealth.CurrentHealth < 25)
                {
                    sway = new Vector2(swayX, swayY);
                }

                if (this._zoomIn)
                {
                    this._currentTimePassed = Math.Min(_currentTimePassed + Time.Instance.DeltaTime, _timeToZoom);

                    float amount = (float)(this._currentTimePassed / this._timeToZoom);
                    this._cam.SetZ(this._cam.GetZFromHeight(MathHelper.Lerp(this._heightWhenNotSlow, this._heightWhenSlow, amount)));

                    if (this._currentTimePassed == this._timeToZoom)
                    {
                        this._currentTimePassed = 0;
                        this._zoomIn = false;    
                    }
                }
                else if(this._zoomOut)
                {
                    this._currentTimePassed = Math.Min(_currentTimePassed + Time.Instance.DeltaTime, _timeToZoom);

                    float amount = (float)(this._currentTimePassed / this._timeToZoom);
                    this._cam.SetZ(this._cam.GetZFromHeight(MathHelper.Lerp(this._heightWhenSlow, this._heightWhenNotSlow, amount)));

                    if (this._currentTimePassed == this._timeToZoom)
                    {
                        this._currentTimePassed = 0;
                        this._zoomOut = false;    
                    }
                }
            }


            Vector2 targetPos = this.Target.Position
                + ((followMouse) ? difference : Vector2.Zero) * 5
                + sway;

            this._cam.Position = Vector2.Lerp(
                this._cam.Position,
                targetPos,
                this.FollowSpeed * Time.Instance.TargetMultiplier
            );

        }
    }
}