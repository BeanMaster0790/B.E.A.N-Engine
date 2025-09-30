using System;
using System.Collections;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Bean;
using Microsoft.Xna.Framework;

namespace DemoGame
{
    class TimeShifter : WorldProp
    {

        public EventHandler<TimeChangeArgs> TimeChange;

        private Color[] _dayColours =
        {
            new Color(255, 120, 90),   // Sunrise  
            new Color(255, 220, 150),  // Afternoon 
            new Color(255, 220, 150), // Keep daytime bright 
            new Color(255, 100, 60),   // Sunset  
            new Color(35, 35, 35),     // Midnight
            new Color(35,35,35) //Keep nighttime dark
        };

        private int[] _timeLenghts =
        {
            15,
            45,
            15,
            15,
            150,
            15
        };

        private int _nextTimeIndex = 0;

        private float _gapToNextTime;

        [DebugServerVariable]
        private float _currentTimePassed;

        [DebugServerVariable]
        public WorldTime worldTime;

        private Color _fromColour;
        private Color _currentColour;

        public override void Start()
        {
            base.Start();

            this.PropID = "TimeShifter";

            this._fromColour = this._dayColours[0];
            this._gapToNextTime = this._timeLenghts[0];

            this._currentColour = this._fromColour;

            this._nextTimeIndex = 4;

            this.IsActive = true;
        }

        public override void Update()
        {
            base.Update();

            this._currentTimePassed += Time.Instance.DeltaTime;

            float amount = this._currentTimePassed / this._gapToNextTime;
            this._currentColour = Color.Lerp(this._fromColour, this._dayColours[this._nextTimeIndex], amount);

            if (this._currentTimePassed >= this._gapToNextTime)
            {
                this._currentTimePassed = 0;
                this._fromColour = this._dayColours[this._nextTimeIndex];

                this._gapToNextTime = this._timeLenghts[this._nextTimeIndex];


                this._nextTimeIndex++;


                if (this._nextTimeIndex >= this._dayColours.Length)
                    this._nextTimeIndex = 0;

                int currentIndex = this._nextTimeIndex - 1;

                bool hasTimeChanged = false;

                switch (currentIndex)
                {
                    case 0:
                        this.worldTime = WorldTime.Sunrise;
                        hasTimeChanged = true;
                        break;
                    case 1:
                        this.worldTime = WorldTime.Day;
                        hasTimeChanged = true;
                        break;
                    case 3:
                        this.worldTime = WorldTime.Sunset;
                        hasTimeChanged = true;
                        break;
                    case 4:
                        this.worldTime = WorldTime.Night;
                        hasTimeChanged = true;
                        break;
                }

                if(hasTimeChanged)
                    this.TimeChange?.Invoke(this, new TimeChangeArgs(this.worldTime, this._timeLenghts[currentIndex]));
            }


            this.Scene.LightingManager.GlobalColour = this._currentColour;
        }
    }

    public enum WorldTime
    {
        Sunrise,
        Day,
        Sunset,
        Night,
    }

    public class TimeChangeArgs : EventArgs
    {
        public WorldTime CurrentTime;

        public int CycleLength;

        public TimeChangeArgs(WorldTime time, int length)
        {
            this.CurrentTime = time;

            this.CycleLength = length; 
        }  
    }
}