using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bean.Graphics.Particles
{
	public class ParticleManager : Addon
	{
		public float EmitFrequency;
		protected float _emitTimer;

		public int SpawnAmout;

		public bool IsPlaying;

		public bool IsLooping;

		protected List<Texture2D> _particleTextures;
		protected List<Particle> _particles;

		public ParticleManager(List<Texture2D> textures) 
		{ 
			this._particleTextures = textures;
		}

		public ParticleManager(Texture2D texture)
		{
			this._particleTextures = new List<Texture2D>
            {
                texture
            };

			this._particles = new List<Particle>();
		}


		public virtual void SpawnParticle()
		{
		}

		public override void Draw(SpriteBatch spriteBatch)
		{

		}

		public override void Update()
		{
			base.Update();

			this._emitTimer += Time.Instance.DeltaTime;

			if(this._emitTimer > EmitFrequency)
			{
				SpawnParticle();

				if(IsLooping)
					this._emitTimer = 0;
				else
					IsPlaying = false;
			}
		}


		public override void Start()
		{
			base.Start();
		}
	}
}
