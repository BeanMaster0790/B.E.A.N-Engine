using Bean;
using Bean.Debug;
using Bean.Graphics;
using Bean.Graphics.Lighting;
using Bean.Sounds;
using Bean.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bean.Scenes
{
    public class Scene
    {
        public Camera Camera;

        public bool _loaded { get; private set; }

        public string Name { get; set; }

        protected List<WorldProp> _sceneProps;

        public SoundManager SoundManager { get; private set; }

        public LightingManager LightingManager { get; private set; }

        public UIScene UIScene { get; private set; }

        public bool RemovalPhase;

        private bool _hasStarted;

        private Timer _serverUpdate;

        public Scene(string name)
        {
            this._sceneProps = new List<WorldProp>();

            this.Name = name;

            this.Camera = new Camera(GraphicsManager.Instance.GraphicsDevice);
            this.Camera.Scene = this;

            this.SoundManager = new SoundManager();

            this.LightingManager = new LightingManager(this.Camera);

            this.UIScene = new UIScene() {Scene = this};

#if DEBUG
            _serverUpdate = new Timer();

            _serverUpdate.StartTimer(0.1f);
#endif
        }

        public virtual void AddToScene(WorldProp prop)
        {
            prop.Scene = this;
            this._sceneProps.Add(prop);

            //DebugServer.SendPropList();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            this.Camera.Draw(spriteBatch, this._sceneProps.ToArray());
        }

        public WorldProp[] GetSceneProps()
        {
            return this._sceneProps.ToArray();
        }

        public WorldProp GetPropWithTag(string tag)
        {
            return this._sceneProps.FirstOrDefault(prop => prop.Tag == tag);
        }

        public WorldProp GetPropWithId(string id)
        {
            return this._sceneProps.FirstOrDefault(prop => prop.PropID == id);
        }

        public WorldProp[] GetPropsWithTag(string tag)
        {
            return this._sceneProps.Where(prop => prop.Tag == tag).ToArray();
        }

        public WorldProp GetPropWithName(string name)
        {
            return this._sceneProps.FirstOrDefault(prop => prop.Name == name);
        }

        public T[] GetPropsWithId<T>(string id)
        {
            List<T> props = new List<T>();

            foreach (Prop prop in this._sceneProps.ToArray())
            {
                if (prop.PropID != id)
                    continue;

                if (prop.GetType() == typeof(T))
                    props.Add((T)Convert.ChangeType(prop, typeof(T)));
            }

            return props.ToArray();
        }


        public virtual void LoadScene(object caller = null)
        {
            if (caller.GetType() != typeof(SceneManager) || caller == null)
                throw new Exception("'LoadScene()' should only be called by the scene manager!");

            if (this._loaded)
                return;

            this._loaded = true;
        }

        public virtual void LateUpdate()
        {
            List<WorldProp> propsToremove = new List<WorldProp>();

            foreach (WorldProp prop in this._sceneProps.ToArray())
            {
                if (prop.IsActive && prop.Started)
                    prop.LateUpdate();

                if (prop.ToRemove)
                    propsToremove.Add(prop);
            }

            this.RemovalPhase = true;

            bool removedObject = false;

            foreach (WorldProp removeProp in propsToremove)
            {
                removeProp.RemoveFromGame();

                this._sceneProps.Remove(removeProp);

                removedObject = true;
            }

            this.RemovalPhase = false;

#if DEBUG
            this._serverUpdate.Update();

            if (removedObject)
                DebugServer.SendPropList();

            if (this._serverUpdate.IsFinished)
            {
                DebugServer.UpdateSelectedProp();
                this._serverUpdate.StartTimer(0.1f);
            }

#endif

        }

        public virtual void UnloadScene(object caller = null)
        {
            if (caller.GetType() != typeof(SceneManager) || caller == null)
                throw new Exception("'UnloadScene()' should only be called by the scene manager!");

            if (SceneManager.Instance.ActiveScene == this)
                throw new Exception("Can't unload active scene.");


            if (this._loaded == false)
                return;

            foreach (Prop prop in this._sceneProps)
            {
                prop.Destroy();
            }

            this.SoundManager.Destroy();

            this._loaded = false;

            this.LateUpdate();
        }

        public virtual void Update()
        {
            this.SoundManager.AudioListener.Position = new Vector3(this.Camera.Position, 0);

            foreach (Prop prop in this._sceneProps.ToArray())
            {
                if (prop.DelayStartTimer != null)
                {
                    prop.DelayStartTimer.Update();

                    if (prop.DelayStartTimer.IsFinished)
                        prop.DelayStartTimer = null;

                    continue;
                }

                prop.Update();
            }

            this.SoundManager.Update();
            this.UIScene.Update();

            if (!this._hasStarted)
                AfterStart();
        }

        public virtual void AfterStart()
        {
            this._hasStarted = true;
        }

    }
}