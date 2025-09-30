using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bean.Graphics.Particles
{
	public class Particle
	{
		public int TextureIndex;

		public Vector2 StartPosition;

		public Vector2 Position;
		public Vector2 Velocity;

		public Color Color;

		public float DeleteTime;
		public float DeleteTimer;

		public float Rotation;
	}
}
