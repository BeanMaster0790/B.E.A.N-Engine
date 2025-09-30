using Microsoft.Xna.Framework.Graphics;
using System;
using Bean.Scenes;
using Bean.Debug;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Linq;
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

        protected bool _started;

        public Scene Scene { get; set; }

        public string Tag { get; set; }

        public bool ToRemove { get; set; }

        private Timer _destroyTimer;

        public Timer DelayStartTimer;

        public Prop()
        {
            Name = "None";
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
                throw new Exception("Use 'prop.Destroy()' To mark an object to be removed from the game at the end of the current frame.");
        }

        public void DestoryAfterSeconds(float seconds)
        {
            this._destroyTimer = new Timer();

            this._destroyTimer.StartTimer(seconds);
        }

        public void DelayStart(float seconds)
        {
            if (this._started)
                throw new Exception("This should be called beofre the object has a chance to start. Before using 'scene.AddProp() is the best way to do this.'");

            this.DelayStartTimer = new Timer();

            this.DelayStartTimer.StartTimer(seconds);
        }

        public virtual void LateUpdate()
        {

        }

        public virtual void Start()
        {
            this._started = true;
        }

        public virtual void Update()
        {
            if (!this._started)
                this.Start();

            if (this._destroyTimer != null)
            {
                this._destroyTimer.Update();

                if (this._destroyTimer.IsFinished)
                    this.Destroy();
            }

        }

        public FieldInfo[] GetFieldsInSceneFile()
        {
            Type type = this.GetType();

            return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
                .Where(f => f.GetCustomAttribute<DebugServerVariable>() != null)
                .ToArray();
        }

        public PropertyInfo[] GetPropertiesInSceneFile()
        {
            Type type = this.GetType();

            var allProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

            return allProperties
                .Where(p => p.CanWrite && p.GetCustomAttribute<DebugServerVariable>() != null)
                .ToArray();
        }

        public void ChangeVariableValue(string fieldName, string value)
        {
            try
            {
                if (UpdateFieldValue(this, fieldName, value))
                    return;
                else if (UpdatePropertyValue(this, fieldName, value))
                    return;

                if (this is WorldProp worldProp)
                {
                    foreach (Addon addon in worldProp.Addons)
                    {
                        if (UpdateFieldValue(addon, fieldName, value))
                            return;
                        else if (UpdatePropertyValue(addon, fieldName, value))
                            return;
                    }
                }
                
                DebugServer.Log("Variable doesn't exist", this, Color.Orange);

            }
            catch (Exception e)
            {
                DebugServer.Log(e.Message, this, Color.Red);
                DebugServer.Log("Invalid Value entered", this, Color.Red);
            }

        }

        private bool UpdatePropertyValue(object target, string fieldName, string value)
        {
            Type type = target.GetType();

            PropertyInfo property = type.GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

            if (property != null && property.CanWrite)
            {
                if (property.PropertyType == typeof(float))
                {
                    property.SetValue(target, float.Parse(value));
                    return true;
                }

                if (property.PropertyType == typeof(int))
                {
                    property.SetValue(target, int.Parse(value));
                    return true;
                }

                if (property.PropertyType == typeof(bool))
                {
                    property.SetValue(target, bool.Parse(value));
                    return true;
                }

                if (property.PropertyType == typeof(Color))
                {
                    property.SetValue(target, JsonConvert.DeserializeObject<Color>(value));
                    return true;
                }

                if (property.PropertyType == typeof(Vector2))
                {
                    string[] strings = value.ToString().Split(',');

                    Vector2 vector = new Vector2(float.Parse(strings[0]), float.Parse(strings[1]));

                    property.SetValue(target, vector);
                    return true;
                }

                property.SetValue(target, value);
                return true;
            }

            return false;
        }

        private bool UpdateFieldValue(object target, string fieldName, string value)
        {
            Type type = target.GetType();

            FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

            if (field != null)
            {
                if (field.FieldType == typeof(float))
                {
                    field.SetValue(target, float.Parse(value));
                    return true;
                }

                if (field.FieldType == typeof(int))
                {
                    field.SetValue(target, int.Parse(value));
                    return true;
                }

                if (field.FieldType == typeof(bool))
                {
                    field.SetValue(target, bool.Parse(value));
                    return true;
                }

                if (field.FieldType == typeof(Color))
                {
                    field.SetValue(target, JsonConvert.DeserializeObject<Color>(value));
                    return true;
                }

                if (field.FieldType == typeof(Vector2))
                {
                    string[] strings = value.ToString().Split(',');

                    Vector2 vector = new Vector2(float.Parse(strings[0]), float.Parse(strings[1]));

                    field.SetValue(target, vector);
                    return true;
                }

                field.SetValue(target, value);
                return true;
            }

            return false;
        }

    }
}
