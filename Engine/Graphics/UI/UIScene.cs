using Microsoft.Xna.Framework;
using Bean.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;
using System;
using Bean.Scenes;
using Bean.Player;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace Bean.UI
{
    public class UIScene
    {
        public int RefrenceHeight {get; set;}

        public int RefernceWidth {get; set;}

        public float RenderScale {get; set;}

        public List<ScreenProp> SceneProps {get; private set;}

        private bool _started;

        private RenderTarget2D _target;

        public Scene Scene;

        public UIScene()
        {
            this.SceneProps = new List<ScreenProp>();

            this.RefrenceHeight = 720;
            this.RefernceWidth = 1280;

            this.RenderScale = 1f;

            GraphicsManager.Instance.GraphicsChanged += (object sender, EventArgs e) =>
            {
                this.RefernceWidth = (int)GraphicsManager.Instance.GetGameResolution().X;
                this.RefrenceHeight = (int)GraphicsManager.Instance.GetGameResolution().Y;
            };
        }

        public void SetTargetSize(object sender, EventArgs args)
        {
            this._target?.Dispose();

            this._target = new RenderTarget2D(GraphicsManager.Instance.GraphicsDevice, RefernceWidth, RefrenceHeight);
        }

        public void AddUIProp(ScreenProp prop)
        {
            prop._UIScene = this; 
            this.SceneProps.Add(prop);
        }

        public void RemoveUIProp(ScreenProp prop)
        {
            this.SceneProps.Remove(prop);

            prop.Destroy();
        }

        public void Start()
        {
            SetTargetSize(this, null);

            GraphicsManager.Instance.GraphicsChanged += SetTargetSize;
        }

        public void Update()
        {
            if (!this._started)
            {
                Start();

                this._started = true;
            }

            ScreenProp[] props = this.SceneProps.Where(i => i.IsActive).ToArray();

            foreach (ScreenProp prop in props)
            {
                if (prop.IsActive)
                    prop.Update();
            }
            
            foreach(ScreenProp prop in props)
            {
                if(prop.IsActive)
                    prop.LateUpdate();
            }
        }

        public RenderTarget2D GetUITexture(SpriteBatch spriteBatch)
        {
            foreach(ScreenProp independentProp in this.SceneProps)
            {
                independentProp.DrawRenderTarget(spriteBatch);
            }

            GraphicsManager.Instance.GraphicsDevice.SetRenderTarget(this._target);
            GraphicsManager.Instance.GraphicsDevice.Clear(new Color(0,0,0, 0));

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.FrontToBack);

            foreach(ScreenProp prop in this.SceneProps)
            {
                if(prop.IsVisable && (prop.Parent == null || prop.Parent is not UIContainer))
                {
                    prop.Draw(spriteBatch);
                }
            }

            spriteBatch.End();

            return this._target;
        }

        public T GetPropWithTag<T>(string tag)
        {
            foreach (Prop prop in this.SceneProps.ToArray())
            {
                if (prop.Tag != tag)
                    continue;

                if (prop.GetType() == typeof(T))
                    return (T)Convert.ChangeType(prop, typeof(T));
            }

            return default(T);
        }

        public T GetPropWithName<T>(string name)
        {
            foreach (Prop prop in this.SceneProps.ToArray())
            {
                if (prop.Name != name)
                    continue;

                if (prop.GetType() == typeof(T))
                    return (T)Convert.ChangeType(prop, typeof(T));
            }

            return default(T);
        }

        public T[] GetPropsWithTag<T>(string tag)
        {
            List<T> props = new List<T>();

            foreach (Prop prop in this.SceneProps.ToArray())
            {
                if (prop.Tag != tag)
                    continue;

                if (prop.GetType() == typeof(T))
                    props.Add((T)Convert.ChangeType(prop, typeof(T)));
            }

            return props.ToArray();
        }

        public T[] GetPropsWithName<T>(string name)
        {
            List<T> props = new List<T>();

            foreach (Prop prop in this.SceneProps.ToArray())
            {
                if (prop.Name != name)
                    continue;

                if (prop.GetType() == typeof(T))
                    props.Add((T)Convert.ChangeType(prop, typeof(T)));
            }

            return props.ToArray();
        }

        public T[] GetAllPropsOfType<T>()
        {
            List<T> props = new List<T>();

            foreach (Prop prop in this.SceneProps.ToArray())
            {
                if (prop.GetType() == typeof(T))
                    props.Add((T)Convert.ChangeType(prop, typeof(T)));
            }

            return props.ToArray();
        }
    }
}