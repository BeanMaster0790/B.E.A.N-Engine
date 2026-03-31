using System.Reflection;
using System.Runtime.CompilerServices;
using Bean.Debug;
using Bean.Graphics;
using Bean.Graphics.Animations;
using Bean.Graphics.Lighting;
using Bean.JsonVariables;
using Bean.PhysicsSystem;
using DemoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace Bean
{
    public class WorldProp : Prop, IJsonParsable<WorldProp>
    {
        public Dictionary<string, Addon> Addons { get; private set; } = new Dictionary<string, Addon>();
        
        private Dictionary<Type, List<Addon>> _addonTypes = new Dictionary<Type, List<Addon>>();
        
        public Transform PropTransform;

        public string LoadedFromFile = "";

        private bool _forceDeleteAddons;
        
        public WorldProp(string name) : base(name)
        {
            if(this.GetAddon<Transform>() == null)
                this.AddAddon(new Transform("Transform"));
            
            this.PropTransform = this.GetAddon<Transform>();
        }

        public WorldProp(string name, Transform propTransform) : base(name)
        {
            this.AddAddon(propTransform);
            
            this.PropTransform = propTransform;
        }

        public void AddAddon(Addon addon)
        {
            if(typeof(Addon) == typeof(Transform))
                throw new ArgumentException("There can only be one transform addon per prop.");

            RequiresAddon attribute = addon.GetType().GetCustomAttribute<RequiresAddon>();

            if (attribute != null && !this._addonTypes.ContainsKey(attribute.AddonType))
            {
                throw new NullReferenceException(
                    $"{addon.GetType().Name} can only be used if {attribute.AddonType.Name} is already on the prop!");
            }
            
            addon.Parent = this;
            
            string attemptedName = addon.Name;
            int i = 1;
            
            while(!this.Addons.TryAdd(addon.Name, addon))
            {
                addon.Name = attemptedName + $" {i}";
                i++;
            }
            
            if(!this._addonTypes.ContainsKey(addon.GetType()))
                this._addonTypes.Add(addon.GetType(), new List<Addon>());
            
            Type t = addon.GetType();
            
            this._addonTypes[t].Add(addon);
        }

        public void RemoveAddon(Addon addon)
        {
            if (addon.ToRemove == false)
            {
                addon.Destroy();
                return;
            }
            
            if (this._forceDeleteAddons == false && !CanDeleteAddon(addon, out Type requiredType))
            {
                DebugServer.LogWarning($"Cannot remove {addon.Name} as it is required by {requiredType.Name}!", this);
                return;
            }
            
            if(!addon.ToRemove)
                DebugServer.LogWarning($"{addon.Name} on prop {this.Name} has not been destroyed yet you're tying to remove it!", this);
            
            this.Addons.Remove(addon.Name);
            
            this._addonTypes[addon.GetType()].Remove(addon);
            
            if(this._addonTypes[addon.GetType()].Count == 0)
                this._addonTypes.Remove(addon.GetType());
        }

        public bool CanDeleteAddon(Addon addon, out Type requiredType)
        {
            if (addon.GetType() == typeof(Transform))
            {
                requiredType = this.GetType();
                return false;
            }
            
            foreach (Type type in _addonTypes.Keys)
            {
                RequiresAddon attribute = type.GetCustomAttribute<RequiresAddon>();

                if (attribute != null && addon.GetType() == attribute.AddonType)
                {
                    requiredType = type;
                    return false;
                }
            }

            requiredType = null;
            return true;
        }

        public override void Update()
        {
            base.Update();

            foreach (Addon addon in this.Addons.Values.ToArray())
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

            foreach (Addon addon in this.Addons.Values.ToArray())
            {
                if (addon.Scene == null)
                    addon.Scene = this.Scene;

                addon.LateUpdate();
            }

        }

        public override void Destroy()
        {
            base.Destroy();

            foreach (Addon addon in this.Addons.Values.ToArray())
            {
                addon.ToRemove = true;
            }
        }

        public override void RemoveFromGame()
        {
            base.RemoveFromGame();

            foreach (string addon in this.Addons.Keys.ToArray())
            {
                Addons[addon].RemoveFromGame();

                this.Addons.Remove(addon);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            foreach (Addon addon in this.Addons.Values)
            {
                addon.Draw(spriteBatch);
            }
        }
        
        /// <summary>
        /// Gets an addon of a set type. In any case where there are multiple of the same addons, it will return the first one it finds (normally the first one you added).
        /// </summary>
        /// <remarks>
        /// If you know the name of the addon you want you should use the Addons dict as you may not get what you are expecting.  
        /// </remarks>
        public T GetAddon<T>() where T : Addon
        {
            if (_addonTypes.TryGetValue(typeof(T), out var list))
                return (T)list[0];

            return null;
        }

        public Addon GetAddon(Type type)
        {
            if (_addonTypes.TryGetValue(type, out var list))
            {
                return list[0];
            }

            return null;
        }

        public T[] GetAddons<T>() where T : Addon
        {
            if (_addonTypes.TryGetValue(typeof(T), out var list))
                return list.Cast<T>().ToArray();

            return Array.Empty<T>();
        }

        public struct WorldPropJson : IBeanJson
        {
            public string Name { get; set; }
            
            public Dictionary<string, string> Addons { get; set; }
        }

        public static WorldProp Parse(string json)
        {
            WorldPropJson? propJsonNull = JsonConvert.DeserializeObject<WorldPropJson>(json);

            if (propJsonNull == null)
                throw new ArgumentException("Invalid Json");
            
            WorldPropJson propJson = (WorldPropJson)propJsonNull;

            WorldProp worldProp = new WorldProp(propJson.Name);
            
            if(propJson.Addons.ContainsKey("Transform"))
                worldProp = new WorldProp(propJson.Name, Transform.Parse(propJson.Addons["Transform"]));
                

            foreach (KeyValuePair<string, string> jsonAddon in propJson.Addons)
            {
                switch (jsonAddon.Key.Replace("TYPECLONE", ""))
                {
                    case "Transform":
                        break;
                    case "Sprite":
                        worldProp.AddAddon(Sprite.Parse(jsonAddon.Value));
                        break;
                    case "AnimationManager":
                        worldProp.AddAddon(AnimationManager.Parse(jsonAddon.Value));
                        break;
                    case "Light":
                        worldProp.AddAddon(Light.Parse(jsonAddon.Value));
                        break;
                    case "YSorter":
                        worldProp.AddAddon(YSorter.Parse(jsonAddon.Value));
                        break;
                    case "PlayerController":
                        worldProp.AddAddon(PlayerController.Parse(jsonAddon.Value));
                        break;
                    case "Collider":
                        worldProp.AddAddon(Collider.Parse(jsonAddon.Value));
                        break;
                    case "PhysicsObject":
                        worldProp.AddAddon(PhysicsObject.Parse(jsonAddon.Value));
                        break;
                    default:
                        throw new NotImplementedException($"Unknown addon {jsonAddon.Key}");
                }
            }
            
            return worldProp;
        }
        

        public void UpdateFromFile()
        {
#if DEBUG
            WorldProp newProp = FileManager.LoadWorldPropFromFile(this.LoadedFromFile);

            Vector2 currentPos = this.PropTransform.Position;

            this._forceDeleteAddons = true;
            
            foreach (Addon addonsValue in this.Addons.Values)
            {
                RemoveAddon(addonsValue);
            }
            
            this._forceDeleteAddons = false;

            foreach (Addon addon in newProp.Addons.Values)
            {
                this.AddAddon(addon);
            }
            
            this.PropTransform = this.GetAddon<Transform>();
            this.PropTransform.Position = currentPos;
#endif
        }

        public void UpdateFromJson(string json)
        {
            WorldPropJson? propJsonNull = JsonConvert.DeserializeObject<WorldPropJson>(json);

            List<string> updatedAddons = new List<string>();
            
            string skippedName = "";

            if (propJsonNull == null)
                throw new ArgumentException("Invalid Json");
            
            WorldPropJson propJson = (WorldPropJson)propJsonNull;
            
            this.Name = propJson.Name;

            foreach (KeyValuePair<string, string> jsonAddon in propJson.Addons)
            {
                BasicBeanJson basicJson = JsonConvert.DeserializeObject<BasicBeanJson>(jsonAddon.Value);

                if (!this.Addons.ContainsKey(basicJson.Name))
                {
                    Console.WriteLine($"Skipping {basicJson.Name}");
                    
                    skippedName =  basicJson.Name;
                    
                    continue;
                }
                
                if (this.Addons[basicJson.Name] != null &&
                    this.Addons[basicJson.Name] is IJsonParsable parsable)
                {
                    parsable.UpdateFromJson(jsonAddon.Value);
                    updatedAddons.Add(basicJson.Name);
                }
            }

            foreach (Addon addon in Addons.Values.ToArray())
            {
                if (!updatedAddons.Contains(addon.Name) && !string.IsNullOrEmpty(skippedName))
                {
                    Console.WriteLine($"changing {addon.Name} to {skippedName}");
                    
                    string oldAddonName = addon.Name;
                    
                    addon.Name = skippedName;
                    
                    this.Addons.Add(addon.Name, addon);
                    this.Addons.Remove(oldAddonName);
                }
            }
        }

        public string ExportJson()
        {
            Dictionary<string, string> addons = new Dictionary<string, string>();
            
            Addon[] addonArray = this.Addons.Values.ToArray();

            for (int i = 0; i < addonArray.Length; i++)
            {
                if (addonArray[i] is IJsonParsable parsable)
                {
                    string attempedName = parsable.GetType().Name;

                    while (addons.ContainsKey(attempedName))
                    {
                        attempedName += "TYPECLONE";
                    }
                    
                    addons.Add(parsable.GetType().Name, parsable.ExportJson());
                }
            }

            WorldPropJson worldProp = new WorldPropJson()
            {
                Name = this.Name,
                Addons = addons
            };

            return JsonConvert.SerializeObject(worldProp);
        }
    }
}