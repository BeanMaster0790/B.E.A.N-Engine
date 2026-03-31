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
	}
}
