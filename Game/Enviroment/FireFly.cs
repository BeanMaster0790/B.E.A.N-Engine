using System;
using Bean;
using Random = Bean.Random;

namespace DemoGame
{
    public class FireFly : Sprite
    {
        private float _originalY;

        private float _originalX;

        private float _wavetimeY;
        private float _wavetimeX;

        public override void Start()
        {
            base.Start();

            this._originalY = this.Position.Y;

            this._originalX = this.Position.X;

            this._wavetimeY = Random.RandomFloat(0, 15);
            this._wavetimeX = Random.RandomFloat(0, 15);
        }

        public override void Update()
        {
            base.Update();

            float waveHeight = 2;

            float waveSpeed = 1f;

            float Y = waveHeight * MathF.Sin(this._wavetimeY / waveSpeed);

            float X = waveHeight * MathF.Sin(this._wavetimeX / waveSpeed);

            this.Position.Y = this._originalY + Y;

            this.Position.X = this._originalX + X;

            this._wavetimeY += Time.Instance.DeltaTime;
            this._wavetimeX -= Time.Instance.DeltaTime;
        }
    }
}