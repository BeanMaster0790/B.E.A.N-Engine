using Bean.Debug;

namespace Bean.Scenes
{
    public class SceneManager
    {
        public static SceneManager Instance = new SceneManager();

        public Scene ActiveScene { get; private set; }

        private Dictionary<string, Scene> _scenes = new Dictionary<string, Scene>();

        SceneManager()
        {
            FileManager.HotReloadRequested += HotReloadRequested;
        }

        private void HotReloadRequested(object sender, EventArgs e)
        {
#if DEBUG
            foreach (WorldProp prop in ActiveScene.GetSceneProps())
            {
                if(!string.IsNullOrEmpty(prop.LoadedFromFile))
                    prop.UpdateFromFile();
            }
#endif
        }

        public void AddNewScene(Scene scene)
        {
            DebugServer.Log("Adding new scene " + scene.Name, null);
            DoesSceneExistError(scene.Name, flip: true);

            _scenes.Add(scene.Name, scene);
        }

        public void LoadScene(string sceneName)
        {
            DebugServer.Log("Loading scene " + sceneName, null);
            DoesSceneExistError(sceneName);

            _scenes[sceneName].LoadScene(caller: this);
        }

        public void RemoveScene(string sceneName)
        {
            DebugServer.Log("Removing scene " + sceneName, null);
            DoesSceneExistError(sceneName);

            _scenes.Remove(sceneName);
        }

        public void SetActiveScene(string sceneName)
        {
            DebugServer.Log("Setting active scene " + sceneName + ". This does not unload the last active scene", null);
            DoesSceneExistError(sceneName);

            Scene scene = _scenes[sceneName];

            if (!scene._loaded)
                throw new Exception("Scene has to be loaded before it can be active");

            this.ActiveScene = scene;
        }


        public void UnloadScene(string sceneName)
        {
            DebugServer.Log("Unloading scene " + sceneName, null);
            DoesSceneExistError(sceneName);

            _scenes[sceneName].UnloadScene(caller: this);
        }

        public void UnloadAllScenes()
        {
            this.ActiveScene = null;
            
            foreach (var scene in _scenes.Values)
            {
                UnloadScene(scene.Name);
            }
        }


        public void DoesSceneExistError(string sceneName, bool flip = false)
        {
            if (this._scenes.TryGetValue(sceneName, out Scene scene))
            {
                if (scene == null && !flip)
                {
                    throw new NullReferenceException($"'{sceneName}' Does not exist! You may need to add your scene using 'AddNewScene()'.");
                }
                else if (scene != null && flip)
                {
                    throw new Exception($"'{sceneName}' Already exists!");
                }
            }
        }
        
        public bool DoesSceneExist(string sceneName)
        {
            if (this._scenes.TryGetValue(sceneName, out Scene scene))
            {
                if (scene == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

    }
}