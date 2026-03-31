using System.Net.Mime;
using Bean.Debug;
using Bean.Graphics;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace Bean
{
    public static class FileManager
    {
        private static string _projectPath = "";
        
        private static string _assetFolderPath = "/Assets/";
        private static string _textureFolderPath = "/Assets/Textures/";
        private static string _fontFolderPath = "/Assets/Fonts/";
        private static string _audioFolderPath = "/Assets/Audio/";
        private static string _sceneFolderPath = "/Assets/Scenes/";
        private static string _tiledMapFolderPath = "/Assets/Maps/";
        private static string _beanObjectsFolderPath = "/Assets/BeanObjects/";

#if DEBUG
        private static string _debugFolderPath = "/Assets/DontShip/";

        private static string _webServerFolderPath = "/Assets/DontShip/ServerFiles/";

        public static string DebugPath
        {
            get
            {
                return Directory.GetCurrentDirectory() + _debugFolderPath;
            }
        }

        public static string ServerPath
        {
            get
            {
                return Directory.GetCurrentDirectory() + _webServerFolderPath;
            }
        }

#endif

        private static Dictionary<string, object> _loadedItems = new Dictionary<string, object>();
        
        private static Dictionary<string, string> _loadedProps = new Dictionary<string, string>();

        public static string AssetsPath
        {
            get
            {
                return Directory.GetCurrentDirectory() + _assetFolderPath;
            }
        }

        public static string TexturePath
        {
            get
            {
                return Directory.GetCurrentDirectory() + _textureFolderPath;
            }
        }

        public static string FontPath
        {
            get
            {
                return Directory.GetCurrentDirectory() + _fontFolderPath;
            }
        }

        public static string AudioPath
        {
            get
            {
                return Directory.GetCurrentDirectory() + _audioFolderPath;
            }
        }

        public static string ScenePath
        {
            get
            {
                return Directory.GetCurrentDirectory() + _sceneFolderPath;
            }
        }

        public static string TiledMapPath
        {
            get
            {
                return Directory.GetCurrentDirectory() + _tiledMapFolderPath;
            }
        }

        public static string BeanObjectsPath
        {
            get 
            {
                return Directory.GetCurrentDirectory() + _beanObjectsFolderPath;
            }
        }

        public static string ProjectPath
        {
            get
            {
                return _projectPath;
            }
        }

        public static EventHandler HotReloadRequested;


        public static void CreateProjectFolders()
        {
#if DEBUG
            HotReloadRequested += (sender, args) =>
            {
                _loadedProps.Clear();
            };
            
            string binName = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Name;

            if (binName == "bin")
            {
                DirectoryInfo projectPath = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent;
                
                _projectPath = projectPath.FullName;

                if (!Directory.Exists(projectPath + _assetFolderPath))
                {
                    Directory.CreateDirectory(projectPath + _assetFolderPath);
                }

                if (!Directory.Exists(projectPath + _textureFolderPath))
                {
                    Directory.CreateDirectory(projectPath + _textureFolderPath);
                }

                if (!Directory.Exists(projectPath + _fontFolderPath))
                {
                    Directory.CreateDirectory(projectPath + _fontFolderPath);
                }

                if (!Directory.Exists(projectPath + _audioFolderPath))
                {
                    Directory.CreateDirectory(projectPath + _audioFolderPath);
                }

                if (!Directory.Exists(projectPath + _sceneFolderPath))
                {
                    Directory.CreateDirectory(projectPath + _sceneFolderPath);
                }

                if (!Directory.Exists(projectPath + _tiledMapFolderPath))
                {
                    Directory.CreateDirectory(projectPath + _tiledMapFolderPath);
                }

                if (!Directory.Exists(projectPath + _debugFolderPath))
                {
                    Directory.CreateDirectory(projectPath + _debugFolderPath);
                }

                if (!Directory.Exists(projectPath + _webServerFolderPath))
                {
                    Directory.CreateDirectory(projectPath + _webServerFolderPath);
                }

                if (!Directory.Exists(projectPath + _beanObjectsFolderPath))
                {
                    Directory.CreateDirectory(projectPath + _beanObjectsFolderPath);
                }


            }

            CreateProjectFiles();
#else

#endif
        }

        public static void CreateProjectFiles()
        {
            if (Directory.Exists(ScenePath))
            {
                string projectScenePath = Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName + _sceneFolderPath);

                if (Directory.GetFiles(projectScenePath).Length == 0)
                {
                    File.Create(projectScenePath + "TestScene.json").Dispose(); // Ensure the file handle is closed after creation
                }
            }
            else
            {
                CreateProjectFolders();
            }
        }

        public static void SaveWorldPropToFile(WorldProp worldProp, string filePath)
        {
            string json = worldProp.ExportJson();

            string destinationFile = _projectPath + _beanObjectsFolderPath + filePath + ".BEAN";
            
            File.WriteAllText(destinationFile, json);
        }

        public static WorldProp LoadWorldPropFromFile(string filePath)
        {
            DebugServer.Log("Loading WorldProp: " + filePath, null);

            string json = "";
            
            if(_loadedProps.ContainsKey(filePath))
                json = _loadedProps[filePath];
            else
            {
#if !DEBUG
                json = File.ReadAllText(BeanObjectsPath + filePath + ".BEAN");
#else

                json = File.ReadAllText(_projectPath + _beanObjectsFolderPath + filePath + ".BEAN");
#endif
            }
            
            if(!_loadedProps.ContainsKey(filePath))
                _loadedProps.Add(filePath, json);
            
            WorldProp prop = WorldProp.Parse(json);
            
            prop.LoadedFromFile =  filePath;
            
            return prop;
        }
        
        public static T GetAddonFromJson<T>(string json)
        {
            var addon = JsonConvert.DeserializeObject<T>(json);
            
            if(addon == null)
                throw new ArgumentException("Invalid Json");
            
            return addon;
        }

        public static T LoadFromFile<T>(string filePath, bool premultiplyAlpha = false, bool loadedOnly = false)
        {
            object obj = null;

            string keyName = filePath + $"({typeof(T).Name})";
            
            if (_loadedItems.ContainsKey(keyName))
            {
                //DebugServer.Log("Using Cached Asset: " + fullPath, null);
                return (T)_loadedItems[keyName];
            }
            
            if (loadedOnly)
                return default(T);
            
            string fullPath = GetFilePath(filePath, typeof(T));

            if (!File.Exists(fullPath))
            {
                if (typeof(T) == typeof(Texture2D))
                {
                    Texture2D texture = new Texture2D(GraphicsManager.Instance.GraphicsDevice, 16, 16);
                    
                    Color[] cols =  new Color[texture.Width * texture.Height];

                    for (int i = 0; i < cols.Length; i++)
                    {
                        if(i % 5 < 5)
                            cols[i] = Color.Purple;
                        else
                        {
                            cols[i] = Color.Black;
                        }
                    }
                    
                    texture.SetData(cols);
                    
                    obj = texture;

                    return (T)obj;
                }
                else
                    throw  new FileNotFoundException($"File not found: {fullPath}");
            }
            
            DebugServer.Log("Loading Asset: " + fullPath, null);
            
            if (typeof(T) == typeof(Texture2D))
            {
                if (!premultiplyAlpha)
                {
                    Texture2D texture = Texture2D.FromFile(GraphicsManager.Instance.GraphicsDevice, fullPath);

                    obj = texture;
                }
                else
                {
                    Texture2D texture = PremultiplyAlpha(GraphicsManager.Instance.GraphicsDevice, Texture2D.FromFile(GraphicsManager.Instance.GraphicsDevice, fullPath));

                    obj = texture;
                }
            }

            else if (typeof(T) == typeof(SoundEffect))
            {
                SoundEffect soundEffect = SoundEffect.FromFile(fullPath);

                obj = soundEffect;
            }

            else if (typeof(T) == typeof(FontSystem))
            {
                FontSystem fontSystem = new FontSystem();

                fontSystem.AddFont(File.ReadAllBytes(fullPath));

                obj = fontSystem;

            }

            else
                throw new NotImplementedException($"Unsupported Type: {typeof(T).Name}");
            
            if(obj != null)
                _loadedItems.Add(keyName, obj);

            return (T)obj;
        }

        private static string GetFilePath(string filePath, Type type)
        {
            string[] splitLocation = (TexturePath + filePath).Split("/");

            string name = splitLocation[splitLocation.Length - 1];

            string location = (GetTypePath(type) + filePath);

            location = location.Remove(location.Length - splitLocation[splitLocation.Length - 1].Length);

            string extention = "";

            foreach (string file in Directory.GetFiles(location))
            {
                if (file.Contains(name))
                {
                    extention = new FileInfo(file).Extension;
                }
            }

            string fullPath = location + "/" + name + extention;
            return fullPath;
        }

        private static string GetTypePath(Type type)
        {
            if (type == typeof(Texture2D))
                return TexturePath;

            if (type == typeof(SoundEffect))
                return AudioPath;

            if (type == typeof(FontSystem))
                return FontPath;

            return AssetsPath;
        }
        
        public static Texture2D PremultiplyAlpha(GraphicsDevice device, Texture2D source)
        {
            
            Color[] data = new Color[source.Width * source.Height];
            source.GetData(data);

            for (int i = 0; i < data.Length; i++)
            {
                byte a = data[i].A;

                data[i].R = (byte)(data[i].R * a / 255);
                data[i].G = (byte)(data[i].G * a / 255);
                data[i].B = (byte)(data[i].B * a / 255);
            }


            Texture2D result = new Texture2D(device, source.Width, source.Height);

            result.SetData(data);

            source.Dispose();

            return result;
        }

        public static void DisposeAsset(string filePath)
        {
            DebugServer.Log("Unloading Asset: " + filePath, null);
            
            IDisposable disposable = _loadedItems[filePath] as IDisposable;
            
            disposable?.Dispose();
            _loadedItems.Remove(filePath);
        }

        public static void UnloadAllAssets()
        {
            foreach (KeyValuePair<string, object> pair in _loadedItems)
            {
                DisposeAsset(pair.Key);
            }
        }

    }
}