using System.Diagnostics;
using System.Reflection;
using Bean.Debug;
using Bean.Editor;
using Bean.Graphics;
using Bean.Graphics.Animations;
using Bean.Graphics.Lighting;
using Bean.JsonVariables;
using Bean.PhysicsSystem;
using Bean.Player;
using Bean.UI;
using DemoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

namespace Bean.Scenes;

public class PropEditor : Scene
{
#if DEBUG
    public WorldProp LoadedProp;

    public Addon SelectedAddon;

    public string CurrentFile;

    private bool _isSaved;

    public PropEditor(string name) : base(name)
    {
    }

    public override void LoadScene(object caller = null)
    {
        base.LoadScene(caller);

        this.Camera.BackgroundColor = new Color(25, 26, 28);

        this.Camera.SetZ(this.Camera.GetZFromHeight(64));

        this.Camera.IsFreeCam = true;

        DebugManager.Instance.ToggleFps(false);

        GenerateTopMenuBar();

        CreateNewProp();
    }

    private void GenerateAddonFieldsList()
    {
        UIAlignContainer rightSideBar = new UIAlignContainer("RightSideBarContainer")
        {
            LocalPosition = new Vector2(0, 25),
            AlignDirection = AlignDirection.Vertical,
            AnchorPoint = UIPropAnchorPoint.TopRight,
            VerticalAlign = VerticalAlign.Top,
            HorizontalAlign = HorizontalAlign.Center,
            Width = 175,
            Height = GraphicsManager.Instance.GetCurentWindowSize().Y - 25,
            Colour = new Color(38, 40, 43),
            Spacing = 10,
            Layer = 0f
        };
        
        UIScene.AddUIProp(EditorUIObjects.CreateFakePaddingBox(rightSideBar));
    
        int i = 0;
        foreach (Addon loadedAddon in this.LoadedProp.Addons.Values)
        {
            Console.WriteLine(loadedAddon.Name);
            
            if (SelectedAddon.Name == loadedAddon.Name) break;
    
            i++;
        }
        
    
        IEnumerable<PropertyInfo> addonProperties = Addon.GetPropertiesWithParseAtt(SelectedAddon.GetType());
        IEnumerable<FieldInfo> addonFields = Addon.GetFieldsWithParseAtt(SelectedAddon.GetType()).ToArray();
    
        foreach (PropertyInfo propertyInfo in addonProperties)
        {
            Tinned attribute = propertyInfo.GetCustomAttribute<Tinned>();
            
            UIAlignContainer fieldContainer = EditorUIObjects.CreateListOption(rightSideBar, attribute.Key, 
                AlignDirection.Vertical, HorizontalAlign.Center, VerticalAlign.Top, EditorUIObjects.BaseColour,
                out UIText fieldName);
            
            Type type = propertyInfo.PropertyType;
            string key = attribute.Key;
            
            UIAlignContainer inputContainer = EditorUIObjects.CreateAddonInputField(type, key, fieldContainer, this.SelectedAddon, this);

            UIScene.AddUIProp(fieldContainer);
            UIScene.AddUIProp(fieldName);
            UIScene.AddUIProp(inputContainer);
        }
        
        foreach (FieldInfo fieldInfo in addonFields)
        {
            Tinned attribute = fieldInfo.GetCustomAttribute<Tinned>();
            
            UIAlignContainer fieldContainer = EditorUIObjects.CreateListOption(rightSideBar, attribute.Key, 
                AlignDirection.Vertical, HorizontalAlign.Center, VerticalAlign.Top, EditorUIObjects.BaseColour,
                out UIText fieldName);
            
            Type type = fieldInfo.FieldType;
            string key = attribute.Key;
            
            UIAlignContainer inputContainer = EditorUIObjects.CreateAddonInputField(type, key, fieldContainer, this.SelectedAddon, this);

            UIScene.AddUIProp(fieldContainer);
            UIScene.AddUIProp(fieldName);
            UIScene.AddUIProp(inputContainer);
        }
        
        UIAlignContainer deleteAddonContainer = EditorUIObjects.CreateListOption(rightSideBar, "Delete Addon",
            alignDirection: AlignDirection.Vertical, HorizontalAlign.Center, VerticalAlign.Top, EditorUIObjects.ButtonColour,
            out UIText deleteAddonText);
    
        deleteAddonContainer.OnLeftClick += (sender, args) => DeleteSelectedAddon();
        
        UIScene.AddUIProp(deleteAddonContainer);
        UIScene.AddUIProp(deleteAddonText);
        
        UIScene.AddUIProp(rightSideBar);
    }

    

    private void DeleteSelectedAddon()
    {
        if (!this.LoadedProp.CanDeleteAddon(this.SelectedAddon, out Type requiredType))
        {
            UIAlignContainer popup = CreatePopupMenu(300, 100, "Cannot Remove");
            
            popup.Spacing = 0;
            popup.VerticalAlign =  VerticalAlign.Center;
            
            UIAlignContainer warningReason = EditorUIObjects.CreateListOption(popup,
                $"{this.SelectedAddon.Name}",
                alignDirection: AlignDirection.Vertical, HorizontalAlign.Center, VerticalAlign.Top,
                EditorUIObjects.BaseColour,
                out UIText reasonText);
            
            this.UIScene.AddUIProp(warningReason);
            this.UIScene.AddUIProp(reasonText);
            
            UIAlignContainer warningTrigger = EditorUIObjects.CreateListOption(popup,
                $"is required by",
                alignDirection: AlignDirection.Vertical, HorizontalAlign.Center, VerticalAlign.Top,
                EditorUIObjects.BaseColour,
                out UIText triggerText);
            
            this.UIScene.AddUIProp(warningTrigger);
            this.UIScene.AddUIProp(triggerText);
            
            UIAlignContainer warningType = EditorUIObjects.CreateListOption(popup,
                $"{requiredType.Name}",
                alignDirection: AlignDirection.Vertical, HorizontalAlign.Center, VerticalAlign.Top,
                EditorUIObjects.BaseColour,
                out UIText typeText);
            
            this.UIScene.AddUIProp(warningType);
            this.UIScene.AddUIProp(typeText);
            
            this.UIScene.AddUIProp(popup);
            
            return;
        }
        
        this.SelectedAddon.Destroy();
                
        SelectAddon(this.LoadedProp.PropTransform);
    }

    public override void Update()
    {
        base.Update();

        if ((InputManager.Instance.IsKeyHeld(Keys.LeftControl) || InputManager.Instance.WasKeyPressed(Keys.LeftControl)) && InputManager.Instance.WasKeyPressed(Keys.S))
        {
            SaveProp();

            this._isSaved = true;
        }

        if (this._isSaved)
        {
            this.UIScene.GetPropWithName<UIAlignContainer>("TopBarContainer").Colour = Color.DarkGreen;
        }
        else
        {
            this.UIScene.GetPropWithName<UIAlignContainer>("TopBarContainer").Colour = Color.DarkRed;
        }
    }

    private void SaveProp()
    {
        FileManager.SaveWorldPropToFile(this.LoadedProp,  (string.IsNullOrEmpty(this.CurrentFile)) ? PromptForSave() : CurrentFile);
    }

    private void LoadProp(string path)
    {
        this.LoadedProp.Destroy();
        
        this.LoadedProp = FileManager.LoadWorldPropFromFile(path);
        this.AddToScene(this.LoadedProp);

        SelectAddon(this.LoadedProp.PropTransform);

        this.CurrentFile = path ;
        this._isSaved = true;
    }

    private void CreateNewProp(bool askToSave = true)
    {
        if (askToSave && this.LoadedProp != null && !this._isSaved)
        {
            UIAlignContainer popup = CreatePopupMenu(300, 100, $"Do you want to save {this.LoadedProp.Name}?");

            popup.VerticalAlign = VerticalAlign.Center;
            
            UIAlignContainer yes = EditorUIObjects.CreateListOption(popup,
                $"Save",
                alignDirection: AlignDirection.Vertical, HorizontalAlign.Center, VerticalAlign.Top,
                EditorUIObjects.ButtonColour,
                out UIText yesText);
            
            UIAlignContainer no = EditorUIObjects.CreateListOption(popup,
                $"Don't Save",
                alignDirection: AlignDirection.Vertical, HorizontalAlign.Center, VerticalAlign.Top,
                EditorUIObjects.ButtonColour,
                out UIText noText);
            
            
            yes.OnLeftClick += (sender, args) =>
            {
                SaveProp();
                popup.Destroy();
                CreateNewProp(false);
            };

            no.OnLeftClick += (sender, args) =>
            {
                popup.Destroy();
                CreateNewProp(false);
            };
            
            this.UIScene.AddUIProp(yes);
            this.UIScene.AddUIProp(yesText);
            
            this.UIScene.AddUIProp(no);
            this.UIScene.AddUIProp(noText);
            
            this.UIScene.AddUIProp(popup);
            
            return;
        }

        this.LoadedProp?.Destroy();
        this.LoadedProp = new WorldProp("Unnamed");
        this.AddToScene(this.LoadedProp);

        SelectAddon(this.LoadedProp.PropTransform);

        this.CurrentFile = "";
    }

    public void SelectAddon(Addon addon)
    {
        UpdateProp();

        this.SelectedAddon = addon;
        this.UIScene.SceneProps.Clear();

        GenerateTopMenuBar();
        GenerateAddonList();
        GenerateAddonFieldsList();
    }

    public void UpdateProp()
    {
        if(this.LoadedProp.Addons == null || this.LoadedProp.Addons.Count == 0)
            return;
        
        this._isSaved = false;
        
        this.LoadedProp.HotReload(CreateWorldPropJson());
    }

    private string CreateWorldPropJson()
    {
        WorldProp.WorldPropJson worldPropJson = new WorldProp.WorldPropJson();
        
         worldPropJson.Name = this.LoadedProp.Name;
        
         List<WorldProp.WorldPropJson.PropAddon> addons = new List<WorldProp.WorldPropJson.PropAddon>();
        
         foreach(Addon addon in this.LoadedProp.Addons.Values)
         {
             WorldProp.WorldPropJson.PropAddon propAddon =
                 new WorldProp.WorldPropJson.PropAddon(addon.GetType().FullName, addon.ExportJson());
                
             addons.Add(propAddon);
         }
        
         worldPropJson.Addons = addons;
        
         return JsonConvert.SerializeObject(worldPropJson);
    }

    private void GenerateTopMenuBar()
    {
        UIAlignContainer topBar = new UIAlignContainer("TopBarContainer")
        {
            AlignDirection = AlignDirection.Horizontal,
            AnchorPoint = UIPropAnchorPoint.TopLeft,
            VerticalAlign = VerticalAlign.Center,
            HorizontalAlign = HorizontalAlign.Left,
            Width = GraphicsManager.Instance.GetCurentWindowSize().X,
            Height = 25,
            Colour = Color.DimGray,
            Spacing = 10,
            Layer = 0.1f,
            ChildrenLayer = 0.2f,
        };

        UIText newPropButton = EditorUIObjects.CreateTopBarOption(topBar, "New Prop");
        UIText loadPropButton = EditorUIObjects.CreateTopBarOption(topBar, "Load Prop");

        loadPropButton.OnLeftClick += (sender, args) =>
        {
            string path = PromptForBEANFile();

            Console.WriteLine($"Selected: {path}");
            
            if(!string.IsNullOrEmpty(path))
                LoadProp(path);
        };

        newPropButton.OnLeftClick += (sender, args) => CreateNewProp();

        UIScene.AddUIProp(topBar);
        UIScene.AddUIProp(newPropButton);
        UIScene.AddUIProp(loadPropButton);
    }

    private string PromptForBEANFile()
    {
        string allowedFolder = FileManager.ProjectPath + "/Assets/BeanObjects/";
        string path = "fill";

        while (true)
        {
            var proc = Process.Start(new ProcessStartInfo
            {
                FileName = "zenity",
                Arguments = $"--file-selection --filename=\"{allowedFolder}/\" --file-filter=\"*.BEAN\"",
                RedirectStandardOutput = true,
                UseShellExecute = false
            });

            path = proc.StandardOutput.ReadToEnd().Trim();

            if (string.IsNullOrEmpty(path))
            {
                break;
            }

            if (!path.StartsWith(allowedFolder))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "zenity",
                    Arguments = $"--error --text=\"Please select a file from (project)/Assets/BeanObjects/**/*.BEAN",
                    UseShellExecute = false
                })?.WaitForExit();
                    
                continue;
            }
                
            FileInfo fileInfo = new FileInfo(path);

            if (fileInfo.Extension != ".BEAN")
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "zenity",
                    Arguments = "--error --text=\"Invalid File Extention!\"",
                    UseShellExecute = false
                })?.WaitForExit();
                    
                continue;
            }
                
            path = path.Replace(allowedFolder, "");
            path = path.Replace(".BEAN", "");
                
            break;
        }

        return path;
    }
    
    private string PromptForSave()
    {
        string allowedFolder = FileManager.ProjectPath + "/Assets/BeanObjects/";
        string path = "fill";

        while (true)
        {
            var proc = Process.Start(new ProcessStartInfo
            {
                FileName = "zenity",
                Arguments = $"--file-selection --save --filename=\"{allowedFolder}/\" --file-filter=\"*.BEAN\"",
                RedirectStandardOutput = true,
                UseShellExecute = false
            });

            path = proc.StandardOutput.ReadToEnd().Trim();

            if (string.IsNullOrEmpty(path))
            {
                break;
            }

            if (!path.StartsWith(allowedFolder))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "zenity",
                    Arguments = $"--error --text=\"Please select a file from (project)/Assets/BeanObjects/**/*.BEAN",
                    UseShellExecute = false
                })?.WaitForExit();
                    
                continue;
            }
                
            FileInfo fileInfo = new FileInfo(path);

            if (fileInfo.Extension != ".BEAN")
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "zenity",
                    Arguments = "--error --text=\"Invalid File Extention!\"",
                    UseShellExecute = false
                })?.WaitForExit();
                    
                continue;
            }
                
            path = path.Replace(allowedFolder, "");
            path = path.Replace(".BEAN", "");
                
            break;
        }

        return path;
    }

    private void GenerateCreateAddonMenu()
    {
        UIAlignContainer addonList = CreatePopupMenu(300, 300, "Create Addon");
        
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(Addon)) || t == typeof(Addon)))
        {
            if(type == typeof(Transform))
                continue;

            UIAlignContainer addonContainer = EditorUIObjects.CreateListOption(addonList, type.Name,
                AlignDirection.Horizontal, HorizontalAlign.Center, VerticalAlign.Top,
                EditorUIObjects.ButtonColour, out UIText text);
            
            addonContainer.OnLeftClick += (sender, args) =>
            {
                Addon addon = CreateAddonFromType(type);
                    
                SelectAddon(addon);
            };
            
            this.UIScene.AddUIProp(addonContainer);
            this.UIScene.AddUIProp(text);
        }
        
        this.UIScene.AddUIProp(addonList);
    }

    private UIAlignContainer CreatePopupMenu(int width, int height, string name)
    {
        UIAlignContainer titleContainer = new UIAlignContainer("PopupTitle")
        {
            Width = width,
            Height = 25,
            AnchorPoint = UIPropAnchorPoint.Center,
            LocalPosition = new Vector2(0, -height / 2 - 20),
            Colour = EditorUIObjects.BaseColour,
            AlignDirection = AlignDirection.Vertical,
            HorizontalAlign =  HorizontalAlign.Center,
            VerticalAlign = VerticalAlign.Top,
            Spacing = 10
        };
        
        UIAlignContainer titleTextContainer =  EditorUIObjects.CreateListOption(titleContainer, name, 
            AlignDirection.Horizontal, HorizontalAlign.Center, VerticalAlign.Top, 
            EditorUIObjects.BaseColour, out UIText titleText);
        
        this.UIScene.AddUIProp(titleContainer);
        this.UIScene.AddUIProp(titleTextContainer);
        this.UIScene.AddUIProp(titleText);
        
        UIAlignContainer closeContainer = new UIAlignContainer("PopupClose")
        {
            Width = width,
            Height = 25,
            AnchorPoint = UIPropAnchorPoint.Center,
            LocalPosition = new Vector2(0, height / 2 + 20),
            Colour = EditorUIObjects.ButtonColour,
            AlignDirection = AlignDirection.Vertical,
            HorizontalAlign =  HorizontalAlign.Center,
            VerticalAlign = VerticalAlign.Top,
            Spacing = 10
        };
        
        UIAlignContainer closeTextContainer =  EditorUIObjects.CreateListOption(closeContainer, "Close", 
            AlignDirection.Horizontal, HorizontalAlign.Center, VerticalAlign.Top, 
            EditorUIObjects.ButtonColour, out UIText closeText);
        
        
        this.UIScene.AddUIProp(closeContainer);
        this.UIScene.AddUIProp(closeTextContainer);
        this.UIScene.AddUIProp(closeText);
        
        UIAlignContainer contentList = new UIAlignContainer("Popup")
        {
            Width = width,
            Height = height,
            AnchorPoint = UIPropAnchorPoint.Center,
            Colour = EditorUIObjects.BaseColour,
            AlignDirection = AlignDirection.Vertical,
            HorizontalAlign =  HorizontalAlign.Center,
            VerticalAlign = VerticalAlign.Top,
            isScrollable = true,
            Spacing = 10
        };

        contentList.OnDestroy += (sender, args) =>
        {
            closeContainer.Destroy();
            titleContainer.Destroy();
        };

        closeContainer.OnLeftClick += (sender, args) => contentList.Destroy();
        
        UIScene.AddUIProp(EditorUIObjects.CreateFakePaddingBox(contentList));
        
        this.UIScene.AddUIProp(contentList);
        
        return contentList;
    }

    private Addon CreateAddonFromType(Type type)
    {
        RequiresAddon attribute = type.GetCustomAttribute<RequiresAddon>();

        if (attribute != null && this.LoadedProp.GetAddon(attribute.AddonType) == null)
        {
            Console.WriteLine(attribute.AddonType.Name);
            CreateAddonFromType(attribute.AddonType);
        }

        Addon addon = Addon.CreateEmpty(type);
        
        this.LoadedProp.AddAddon(addon);

        return addon;
    }

    private void GenerateAddonList()
    {
        UIAlignContainer leftSideBar = new UIAlignContainer("LeftSideBarContainer")
        {
            LocalPosition = new Vector2(0, 25),
            AlignDirection = AlignDirection.Vertical,
            AnchorPoint = UIPropAnchorPoint.TopLeft,
            VerticalAlign = VerticalAlign.Top,
            HorizontalAlign = HorizontalAlign.Center,
            Width = 225,
            Height = GraphicsManager.Instance.GetCurentWindowSize().Y - 25,
            Colour = new Color(38, 40, 43),
            Spacing = 10,
            Layer = 0f
        };

        UIScene.AddUIProp(EditorUIObjects.CreateFakePaddingBox(leftSideBar));

        UIAlignContainer propName = new UIAlignContainer($"PropName")
        {
            Width = leftSideBar.Width - 5,
            Height = 25,
            Colour = new Color(45, 73, 73),
            Parent = leftSideBar,
            AlignDirection = AlignDirection.Horizontal,
            HorizontalAlign = HorizontalAlign.Center,
            Spacing = 10,
            isScrollable = true
        };

        UIInputText propNameInput = new UIInputText($"PropNameInput")
        {
            Width = 20,
            Height = 25,
            FontSize = 24,
            Text = this.LoadedProp.Name,
            PlaceHolderText = "Unnamed",
            InputtedText = this.LoadedProp.Name,
            Parent = propName,
            Colour = Color.White
        };

        propNameInput.OnTextInput += (sender, args) =>
        {
            if (string.IsNullOrEmpty(propNameInput.InputtedText))
            {
                this.LoadedProp.Name = "Unnamed";
                this.UpdateProp();
                return;
            }

            this.LoadedProp.Name = propNameInput.InputtedText;
            this.UpdateProp();
        };

        UIScene.AddUIProp(propName);
        UIScene.AddUIProp(propNameInput);

        foreach (KeyValuePair<string, Addon> propAddon in this.LoadedProp.Addons)
        {
            UIAlignContainer addonContainer = EditorUIObjects.CreateListOption(leftSideBar, propAddon.Key, 
                AlignDirection.Horizontal, HorizontalAlign.Center, VerticalAlign.Top, EditorUIObjects.BaseColour, 
                out UIText addonText);

            addonContainer.OnLeftClick += (sender, args) => SelectAddon(propAddon.Value);
            
            UIScene.AddUIProp(addonContainer);
            UIScene.AddUIProp(addonText);
        }

        UIScene.AddUIProp(leftSideBar);
        
        UIAlignContainer createAddonContainer = EditorUIObjects.CreateListOption(leftSideBar, "Create Addon", 
            AlignDirection.Horizontal, HorizontalAlign.Center, VerticalAlign.Top, EditorUIObjects.BaseColour, 
            out UIText createAddonText);

        UIScene.AddUIProp(createAddonContainer);
        UIScene.AddUIProp(createAddonText);

        createAddonContainer.OnLeftClick += (sender, args) => GenerateCreateAddonMenu();
    }
    #endif
}
