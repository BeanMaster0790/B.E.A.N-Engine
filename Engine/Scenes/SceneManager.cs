using System;
using System.Collections.Generic;

namespace Bean.Scenes
{
    public class SceneManager
    {
        public static SceneManager Instance = new SceneManager();

        public Scene ActiveScene { get; private set; }

        private Dictionary<string, Scene> _scenes = new Dictionary<string, Scene>();

        public void AddNewScene(Scene scene)
        {
            DoesSceneExist(scene.Name, flip: true);

            _scenes.Add(scene.Name, scene);
        }

        public void LoadScene(string sceneName)
        {
            DoesSceneExist(sceneName);

            _scenes[sceneName].LoadScene(caller: this);
        }

        public void RemoveScene(string sceneName)
        {
            DoesSceneExist(sceneName);

            _scenes.Remove(sceneName);
        }

        public void SetActiveScene(string sceneName)
        {
            DoesSceneExist(sceneName);

            Scene scene = _scenes[sceneName];

            if (!scene._loaded)
                throw new Exception("Scene has to be loaded before it can be active");

            this.ActiveScene = scene;
        }


        public void UnloadScene(string sceneName)
        {
            DoesSceneExist(sceneName);

            _scenes[sceneName].UnloadScene(caller: this);
        }


        public void DoesSceneExist(string sceneName, bool flip = false)
        {
            if (this._scenes.TryGetValue(sceneName, out Scene scene))
            {
                if (scene == null && !flip)
                {
                    throw new NullReferenceException($"'{sceneName}' Does not excit! You may need to add your scene using 'AddNewScene()'.");
                }
                else if (scene != null && flip)
                {
                    throw new Exception($"'{sceneName}' Already exists!");
                }
            }
        }

    }
}