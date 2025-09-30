using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bean.Debug
{
	internal class SceneProp
	{
		public string PropID {get; set;}
		public string Name { get; set; }

		public string Tag { get; set; }

		public bool IsVisable { get; set; }

		public bool IsActive { get; set; }

		public string PropType { get; set; }

		public Dictionary<string, object> PropVariables { get; set; }

		public List<PropAddon> propAddons { get; set; }
	}

	internal class PropAddon
	{
		public string AdonId { get; set; }
		public string AddonType { get; set; }
		public Dictionary<string, object> AddonVariables { get; set; }
	}

}

namespace Bean
{

	[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class DebugServerVariable : Attribute {}
}