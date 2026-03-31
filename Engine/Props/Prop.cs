using Microsoft.Xna.Framework.Graphics;
using Bean.Scenes;
using Bean.Debug;
using System.Reflection;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Data.Common;
using System.Reflection.Metadata;

namespace Bean
{
    public class Prop
    {
        public string PropID { get; protected set; }

        public string Name { get; set; }

        public virtual bool IsVisable { get; set; } = true;
        public virtual bool IsActive { get; set; } = true;

        public bool IsTemp;

        public bool Started;

        public Scene Scene { get; set; }

        public string Tag { get; set; }

        public bool ToRemove { get; set; }

        private Timer _destroyTimer;

        public Timer DelayStartTimer;

        public Prop(string name)
        {
            Name = name;
            Tag = "None";

            if(!this.IsTemp)
                this.PropID = PropIds.GeneratePropId();
        }

        public virtual void Draw(SpriteBatch spriteBatch) { }

        public virtual void Destroy()
        {
            this.ToRemove = true;
            this.IsVisable = false;
        }

        public virtual void RemoveFromGame()
        {
            if (!this.Scene.RemovalPhase)
                DebugServer.LogWarning("Use 'prop.Destroy()' To mark an object to be removed from the game at the end of the current frame.", this);
        }

        public void DestoryAfterSeconds(float seconds)
        {
            this._destroyTimer = new Timer();

            this._destroyTimer.StartTimer(seconds);
        }

        public void DelayStart(float seconds)
        {
            if (this.Started)
                throw new Exception("This should be called before the object has a chance to start. Before using 'scene.AddProp() is the best way to do this.'");

            this.DelayStartTimer = new Timer();

            this.DelayStartTimer.StartTimer(seconds);
        }

        public virtual void LateUpdate()
        {
            
        }

        public virtual void Start()
        {
            this.Started = true;
        }

        public virtual void Update()
        {
            if (!this.Started)
                this.Start();

            if (this._destroyTimer != null)
            {
                this._destroyTimer.Update();

                if (this._destroyTimer.IsFinished)
                    this.Destroy();
            }

        }
    }
}
