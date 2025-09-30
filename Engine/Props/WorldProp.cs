
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bean
{
    public class WorldProp : Prop 
    {
        public List<Addon> Addons = new List<Addon>();

        [DebugServerVariable]
        public Vector2 Position;

        [DebugServerVariable]
        public float Rotation; 

        [DebugServerVariable]
        public float Layer = 0;

        [DebugServerVariable]
        public float Scale = 1;

        public WorldProp() : base()
        {

        }

        public override void Start()
        {
            base.Start();
        }

        public virtual void AddAddon(Addon addon)
        {
            addon.Parent = this;

            this.Addons.Add(addon);
        }

        public override void Update()
        {
            base.Update();

            foreach (Addon addon in this.Addons)
            {
                if (addon.Scene == null)
                    addon.Scene = this.Scene;

                if(addon.IsActive)
                    addon.Update();
            }
        }

        public override void LateUpdate()
        {
            base.LateUpdate();

            foreach (Addon addon in this.Addons)
            {
                if (addon.Scene == null)
                    addon.Scene = this.Scene;

                addon.LateUpdate();
            }

        }

        public override void Destroy()
        {
            base.Destroy();

            foreach (Addon addon in this.Addons.ToArray())
            {
                addon.ToRemove = true;
            }
        }

        public override void RemoveFromGame()
        {
            base.RemoveFromGame();

            foreach (Addon addon in this.Addons.ToArray())
            {
                addon.RemoveFromGame();

                this.Addons.Remove(addon);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            foreach (Addon addon in this.Addons.ToArray())
            {
                addon.Draw(spriteBatch);
            }
        }

        public T GetAddon<T>()
        {
            foreach (Addon addon in this.Addons)
            {
                if (addon.GetType() == typeof(T))
                    return (T)Convert.ChangeType(addon, typeof(T));
            }
            return default(T);
        }

        public T[] GetAddons<T>()
        {
            List<T> list = new List<T>();

            foreach (Addon addon in this.Addons)
            {
                if (addon.GetType() == typeof(T))
                    list.Add((T)Convert.ChangeType(addon, typeof(T)));
            }

            return list.ToArray();
        }
    }
}