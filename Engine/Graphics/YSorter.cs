using Bean.JsonVariables;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Bean.Graphics
{
    public class YSorter : Addon, IJsonParsable<YSorter>
    {

        private float _originalLayer;

        [DebugServerVariable]
        public float YOrigin;

        public YSorter(string name) : base(name)
        {
        }

        public override void Start()
        {
            base.Start();

            this._originalLayer = this.Parent.PropTransform.Layer;

            if (this.YOrigin == 0)
            {
                Sprite sprite = this.Parent.GetAddon<Sprite>();
                if (sprite is not null)
                {
                    this.YOrigin = sprite.Origin.Y;
                }
            }
        }

        public override void LateUpdate()
        {
            base.LateUpdate();

            float yPos = this.Scene.Camera.WorldToScreen(this.Parent.PropTransform.Position).Y;

            float yAfterOrigin = yPos - this.YOrigin;

            yAfterOrigin = MathF.Floor(yAfterOrigin);

            this.Parent.PropTransform.Layer = yAfterOrigin / 10000;

            if (this.Parent.PropTransform.Layer < 0.005f)
                this.Parent.PropTransform.Layer = 0.005f;
        }

        public static YSorter Parse(string json)
        {
            BasicBeanJson? jsonNull = JsonConvert.DeserializeObject<BasicBeanJson>(json);
            
            if(jsonNull == null)
                throw new ArgumentException("Invalid JSON");
            
            BasicBeanJson jsonObj = (BasicBeanJson)jsonNull;
            
            return new YSorter(jsonObj.Name);
        }

        public void UpdateFromJson(string json)
        {
            BasicBeanJson? jsonNull = JsonConvert.DeserializeObject<BasicBeanJson>(json);
            
            if(jsonNull == null)
                throw new ArgumentException("Invalid JSON");
            
            BasicBeanJson jsonObj = (BasicBeanJson)jsonNull;
            
            this.Name = jsonObj.Name;
        }

        public string ExportJson()
        {
            BasicBeanJson json = new BasicBeanJson()
            {
                Name = this.Name,
            };
            
            return JsonConvert.SerializeObject(json);
        }
    }
}