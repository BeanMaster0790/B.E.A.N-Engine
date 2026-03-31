using Bean.PhysicsSystem;
using Microsoft.Xna.Framework;
using System.Text;

namespace Bean.Debug
{
	public interface IDebuggable
	{
		public bool ShowDebugInfo { get; set; }

		public List<string> DebugValueNames { get;}

		public void AddDebugValue(string name);

		public Vector2 GetDebugDrawPosition();

	}
}
