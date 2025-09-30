using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bean.Debug
{
	public class DebugValue
	{
		public string Name;

		public int? IntValue;
		public float? FloatValue;
		public string StringValue;

		public DebugValue(string name, ref int value)
		{
			this.Name = name;
			this.IntValue = value;	
		}

		public DebugValue(string name, float value)
		{
			this.Name = name;
			this.FloatValue = value;
		}

		public DebugValue(string name, ref string value)
		{
			this.Name = name;
			this.StringValue = value;
		}
	}
}
