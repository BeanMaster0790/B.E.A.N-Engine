using System;
using Microsoft.Xna.Framework;

namespace Bean.Graphics
{
    public class YSorter : Addon
    {

        private float _originalLayer;

        [DebugServerVariable]
        public float YOrigin;

        public override void Start()
        {
            base.Start();

            this._originalLayer = this.Parent.Layer;

            if (this.YOrigin == 0)
            {
                if (this.Parent is Sprite sprite)
                {
                    this.YOrigin = sprite.GetOrigin().Y;   
                }    
            }
        }

        public override void LateUpdate()
        {
            base.LateUpdate();

            float yPos = this.Scene.Camera.WorldToScreen(this.Parent.Position).Y;

            float yAfterOrigin = yPos + this.YOrigin;

            yAfterOrigin = MathF.Floor(yAfterOrigin);

            this.Parent.Layer = yAfterOrigin / 10000;

            if (this.Parent.Layer < 0.005f)
                this.Parent.Layer = 0.005f;
        }

    }
}