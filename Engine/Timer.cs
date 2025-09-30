namespace Bean
{
    public class Timer : Addon
    {
        [DebugServerVariable]
        private float _duration;

        [DebugServerVariable]
        private float _timePassed;

        [DebugServerVariable]
        public bool IsFinished;

        public override void Start()
        {
            base.Start();
        }

        public void StartTimer(float duration)
        {
            this.IsFinished = false;

            this._duration = duration;

            this._timePassed = 0;

            this.IsActive = true;
        }

        public void PauseTimer()
        {
            this.IsActive = false;    
        }

        public void FinishTimer()
        {
            this.IsFinished = true;

            this.IsActive = false;
        }

        public override void Update()
        {
            base.Update();

            if (this.IsActive)
            {
                this._timePassed += Time.Instance.DeltaTime;

                if (this._timePassed >= this._duration)
                {
                    FinishTimer();    
                }
            }
        }
    }
}