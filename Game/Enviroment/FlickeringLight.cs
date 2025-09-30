using System.Reflection.Metadata;
using Bean;
using Bean.Graphics.Lighting;
using Microsoft.Xna.Framework;

namespace DemoGame
{
    public class FlickeringLight : Light
    {

        [DebugServerVariable]
        private int _maxDistance;

        [DebugServerVariable]
        private int _minDistance;

        [DebugServerVariable]
        private int _steps;

        private int _currentStep;

        [DebugServerVariable]
        private float _stepDuration;

        private int _distanceChangePerStep;

        private bool _isIncreasing;

        private Timer _stepTimer;

        public FlickeringLight(float intencity, int distance, int minDistance, int numberOfSteps, float stepDuration, Color colour) : base(intencity, distance, colour)
        {
            this._minDistance = minDistance;
            this._maxDistance = distance;

            this._steps = numberOfSteps;

            this._stepDuration = stepDuration;

            int distanceDifference = distance - minDistance;

            this._distanceChangePerStep = distanceDifference / this._steps;
        }

        public override void Start()
        {
            base.Start();

            this._stepTimer = new Timer();

            this._stepTimer.StartTimer(this._stepDuration);
        }

        public override void Update()
        {
            base.Update();

            this._stepTimer.Update();


            if (this._stepTimer.IsFinished)
            {
                this._stepTimer.StartTimer(this._stepDuration);

                if (this._currentStep <= this._steps && !this._isIncreasing)
                {
                    this._currentStep++;

                    this.Distance -= this._distanceChangePerStep;
                }
                else if (this._currentStep > this._steps || (this._isIncreasing && this._currentStep > 0))
                {
                    this._isIncreasing = true;

                    this._currentStep--;

                    this.Distance += this._distanceChangePerStep;
                }
                else if (this._currentStep <= 0)
                {
                    this._isIncreasing = false;
                }
            }

        }
    }
}