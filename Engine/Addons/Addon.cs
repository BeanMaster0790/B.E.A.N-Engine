using Bean.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using Bean.JsonVariables;

namespace Bean
{
	public class Addon : Prop
	{
		public WorldProp Parent;

		public Addon(string name) : base(name)
		{
			
		}

		public override void Destroy()
		{
			base.Destroy();
			
			this.Parent.RemoveAddon(this);
        }

		public string ExportJson()
		{
			Dictionary<string, string> jsonValues = new Dictionary<string, string>();
            
			Type type = this.GetType();

			var fieldsWithParseAtt = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance)
				.Where(f => f.IsDefined(typeof(Tinned), true));
			
			var propsWithParseAtt = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance)
				.Where(f => f.IsDefined(typeof(Tinned), true));

			foreach (FieldInfo field in fieldsWithParseAtt)
			{
				Tinned attribute = field.GetCustomAttribute<Tinned>();

				if (attribute.customParseType == null)
				{
						jsonValues.Add(attribute.Key, JsonConvert.SerializeObject(field.GetValue(this)));
	

				}
				else
				{
					ICustomJsonParse converter =  Activator.CreateInstance(attribute.customParseType) as  ICustomJsonParse;
					
					jsonValues.Add(attribute.Key, converter.ToJson(field.GetValue(this)));
				}
			}
			
			foreach (PropertyInfo prop in propsWithParseAtt)
			{
				Tinned attribute = prop.GetCustomAttribute<Tinned>();

				if (prop.CanRead)
				{
					if (attribute.customParseType == null)
					{
							jsonValues.Add(attribute.Key, JsonConvert.SerializeObject(prop.GetValue(this)));

					}
					else
					{
						ICustomJsonParse converter =  Activator.CreateInstance(attribute.customParseType) as  ICustomJsonParse;
					
						jsonValues.Add(attribute.Key, converter.ToJson(prop.GetValue(this)));
					}
				}
			}

			return JsonConvert.SerializeObject(jsonValues);
		}

		public void UpdateFromJson(string json)
		{
			var jsonValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
			
			Type type = this.GetType();
			
			var fieldsWithParseAtt = GetFieldsWithParseAtt(type);
            
			var propsWithParseAtt = GetPropertiesWithParseAtt(type);
			
			Addon.SetValues(fieldsWithParseAtt, propsWithParseAtt, this, jsonValues);
			
			this.InvokeRefryMethods();
		}

		public void InvokeRefryMethods()
		{
			Type type = this.GetType();
			
			var fieldsWithParseAtt = GetFieldsWithParseAtt(type);
            
			var propsWithParseAtt = GetPropertiesWithParseAtt(type);
			
			var hotReloadMethords = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance)
				.Where(m => m.IsDefined(typeof(Refry), true));

			foreach (var hotReloadMethord in hotReloadMethords)
			{
				Refry attribute =  hotReloadMethord.GetCustomAttribute<Refry>();
				
				object[] args = new object[hotReloadMethord.GetParameters().Length];

				int i = 0;
				foreach (string argKey in attribute.ArgKeys)
				{
					if(fieldsWithParseAtt.Any(f => f.GetCustomAttribute<Tinned>().Key == argKey))
						args[i] = fieldsWithParseAtt.First(f => f.GetCustomAttribute<Tinned>().Key == argKey).GetValue(this);
					
					else if(propsWithParseAtt.Any(p => p.GetCustomAttribute<Tinned>().Key == argKey))
						args[i] = propsWithParseAtt.First(p => p.GetCustomAttribute<Tinned>().Key == argKey).GetValue(this);
						
					i++;
				}
				
				hotReloadMethord.Invoke(this, args);
			}
		}

		public static IEnumerable<FieldInfo> GetFieldsWithParseAtt(Type type)
		{
			return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance)
				.Where(f => f.IsDefined(typeof(Tinned), true));
		}

		public static IEnumerable<PropertyInfo> GetPropertiesWithParseAtt(Type type)
		{
			return type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance)
				.Where(f => f.IsDefined(typeof(Tinned), true));
		}

		public static Addon Parse(string json, Type type)
		{
            var jsonValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

			IEnumerable<FieldInfo> fieldsWithParseAtt = GetFieldsWithParseAtt(type);
            
            IEnumerable<PropertyInfo> propsWithParseAtt = GetPropertiesWithParseAtt(type);
            
            var args = GenerateConstructorArguments(type, fieldsWithParseAtt, propsWithParseAtt, jsonValues);

            Object obj = type.GetConstructors().First(c => c.GetParameters().Length == args.Length).Invoke(args);

            SetValues(fieldsWithParseAtt, propsWithParseAtt, obj, jsonValues);
            
            return obj as Addon;
		}

		public static Addon CreateEmpty(Type type)
		{
			ParameterInfo[] paramInfos = type.GetConstructors()[0].GetParameters();
			
			Object[] args = new Object[paramInfos.Length];

			int i = 0;
			foreach (ParameterInfo param in paramInfos)
			{
				Type paramType = param.ParameterType;

				if (paramType == typeof(string))
				{
					args[i] = "string.Empty";
				}
				else if (paramType.IsValueType)
				{
					args[i] = Activator.CreateInstance(param.ParameterType);
				}
				else
				{
					args[i] = null;
				}

				i++;
			}

			args[0] = type.Name;
			return type.GetConstructors()[0].Invoke(args) as Addon;
		}

		private static void SetValues(IEnumerable<FieldInfo> fieldsWithParseAtt, IEnumerable<PropertyInfo> propsWithParseAtt, object obj, Dictionary<string, string> jsonValues)
		{
			foreach (FieldInfo field in fieldsWithParseAtt)
			{
				Tinned attribute = field.GetCustomAttribute<Tinned>();

				if (!attribute.ConstructorValue)
				{
					if(attribute.customParseType == null)
						field.SetValue(obj, JsonConvert.DeserializeObject(jsonValues[attribute.Key], field.FieldType));
					else
					{
						ICustomJsonParse converter =  Activator.CreateInstance(attribute.customParseType) as  ICustomJsonParse;
						
						field.SetValue(obj, converter.FromJson(jsonValues[attribute.Key]));
					}
				}
			}
			
			foreach (PropertyInfo prop in propsWithParseAtt)
			{
				Tinned attribute = prop.GetCustomAttribute<Tinned>();

				if (!attribute.ConstructorValue && prop.CanWrite)
				{
					if(attribute.customParseType == null)
						prop.SetValue(obj, JsonConvert.DeserializeObject(jsonValues[attribute.Key], prop.PropertyType));
					else
					{
						ICustomJsonParse<object> converter =  Activator.CreateInstance(attribute.customParseType) as  ICustomJsonParse<object>;
						
						prop.SetValue(obj, converter.FromJson(jsonValues[attribute.Key]));
					}
					
				}
			}
		}

		private static object[] GenerateConstructorArguments(Type type, IEnumerable<FieldInfo> fieldsWithParseAtt, IEnumerable<PropertyInfo> propsWithParseAtt,Dictionary<string, string> jsonValues)
		{
			int argumentCount = fieldsWithParseAtt.Where(f => f.GetCustomAttribute<Tinned>().ConstructorValue).Count();
			argumentCount += propsWithParseAtt.Where(p => p.GetCustomAttribute<Tinned>().ConstructorValue).Count();

			object[] args = new object[argumentCount];
            
			foreach (var field in fieldsWithParseAtt)
			{
				Tinned attribute = field.GetCustomAttribute<Tinned>();
                
				if (attribute.ConstructorValue)
				{
					if(attribute.ConstructorIndex == -1)
						throw new ArgumentException($"ParseValue on {field.Name}({attribute.Key}) on type {field.DeclaringType} is said to be a constructor but isn't assigned an index.");

					if (attribute.ConstructorIndex == 0 && attribute.Key != "Name")
						throw new ArgumentException(
							$"ParseValue on {field.Name}({attribute.Key}) on type {field.DeclaringType} is said to be argument 0 but this is reserved for prop names.");
                    
					if(attribute.customParseType == null)
						args[attribute.ConstructorIndex] = JsonConvert.DeserializeObject(jsonValues[attribute.Key],  field.FieldType);
					else
					{
						ICustomJsonParse converter =  Activator.CreateInstance(attribute.customParseType) as  ICustomJsonParse;
						
						args[attribute.ConstructorIndex] = converter.FromJson(jsonValues[attribute.Key]);
					}
					
				}
			}
			
			foreach (var prop in propsWithParseAtt)
			{
				Tinned attribute = prop.GetCustomAttribute<Tinned>();
                
				if (attribute.ConstructorValue)
				{
					if(attribute.ConstructorIndex == -1)
						throw new ArgumentException($"ParseValue on {prop.Name}({attribute.Key}) on type {prop.DeclaringType} is said to be a constructor but isn't assigned an index.");

					if (attribute.ConstructorIndex == 0 && attribute.Key != "Name")
						throw new ArgumentException(
							$"ParseValue on {prop.Name}({attribute.Key}) on type {prop.DeclaringType} is said to be argument 0 but this is reserved for prop names.");
                    
					if(attribute.customParseType == null)
						args[attribute.ConstructorIndex] = JsonConvert.DeserializeObject(jsonValues[attribute.Key],  prop.PropertyType);
					else
					{
						ICustomJsonParse converter =  Activator.CreateInstance(attribute.customParseType) as  ICustomJsonParse;
						
						args[attribute.ConstructorIndex] = converter.FromJson(jsonValues[attribute.Key]);
					}
				}
			}

			return args;
		}
	}
}
