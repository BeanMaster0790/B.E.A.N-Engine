using System;
using System.Collections.Generic;
using System.IO;
using Bean.Graphics;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Bean
{
    public static class FileManager
    {
        private static string _assetFolderPath = "/Assets/";
        private static string _textureFolderPath = "/Assets/Textures/";
        private static string _fontFolderPath = "/Assets/Fonts/";
        private static string _audioFolderPath = "/Assets/Audio/";
        private static string _sceneFolderPath = "/Assets/Scenes/";
        private static string _tiledMapFolderPath = "/Assets/Maps/";

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


        public static void CreateProjectFolders()
        {
#if DEBUG
            string binName = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Name;

            if (binName == "bin")
            {
                DirectoryInfo projectPath = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent;

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

        public static T LoadFromFile<T>(string filePath, bool premultiplyAlpha = false)
        {
            object obj = null;

            string fullPath = GetFilePath(filePath, typeof(T));

            if (_loadedItems.ContainsKey(fullPath))
                return (T)_loadedItems[fullPath];

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

            _loadedItems.Add(fullPath, obj);

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

    }
}