using Bean.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bean
{
	public class Addon : Prop
	{
		public WorldProp Parent;

		public override void Destroy()
		{
			base.Destroy();

			this.Parent.Addons.Remove(this);
        }
	}
}
