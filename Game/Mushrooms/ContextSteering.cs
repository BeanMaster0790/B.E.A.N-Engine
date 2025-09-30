using Bean;
using Bean.Debug;
using Bean.Graphics;
using Bean.PhysicsSystem;
using DemoGame;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Random = Bean.Random;

namespace DemoGame
{
	public class ContextSteering : Addon
	{
		public Sprite Target
		{
			get
			{
				return (this.IsStuck) ? this._stuckTarget : this._target;
			}
			set
			{
				this._target = value;
			}
		}

		private Sprite _target;

		private Sprite _stuckTarget;

		public Vector2 MoveVector;

		private string[] _avoidTags;
		private string[] _preferAvoidTags;

		private int _lookAheadRange;

		private int _numberOfDirections;

		private Vector2[] _directions;

		private float[] _dangerVectors;
		private float[] _interestVectors;
		private float[] _finalVectors;

		private Vector2 _directionToTarget;

		public bool CanSeeTarget;

		public bool TargetReached;

		[DebugServerVariable]
		public bool IsStuck;


		public ContextSteering(int lookAhead, int numberOfDirections, string[] avoidTags, string[] preferAvoid, Sprite target)
		{
			this._lookAheadRange = lookAhead;

			this._avoidTags = avoidTags;

			this._preferAvoidTags = preferAvoid;

			this.Target = target;

			this._directions = new Vector2[numberOfDirections];

			this._dangerVectors = new float[numberOfDirections];

			this._interestVectors = new float[numberOfDirections];

			this._finalVectors = new float[numberOfDirections];

			this._numberOfDirections = numberOfDirections;
		}

		public override void Start()
		{
			base.Start();

			this._directions = this.GetVectorDirections(this._directions.Length);
		}

		int stuckCount = 0;

		public override void Update()
		{
			base.Update();

			if (Target == null)
			{
				this.MoveVector = Vector2.Zero;
				return;
			}
			if (Vector2.Distance(this.Parent.Position, this.Target.Position) < 16)
			{
				if (this.IsStuck)
				{
					this.IsStuck = false;

					return;	
				}

				this.TargetReached = true;
				this.MoveVector = Vector2.Zero;
				return;
			}

			DebugManager.Instance.DrawCircle(this.Target.Position, 24, Color.CornflowerBlue, true);

			this._directionToTarget = this.Target.Position - this.Parent.Position;

			this._directionToTarget.Normalize();

			this._interestVectors = GenerateIntrestVectors();

			this._dangerVectors = GenerateDangerVectors();

			this._finalVectors = GenerateFinalVectors();

			Vector2 moveVector = Vector2.Zero;

			for (int i = 0; i < this._numberOfDirections; i++)
			{
				moveVector += this._directions[i] * this._finalVectors[i];
			}

			if (moveVector != Vector2.Zero)
				moveVector.Normalize();

			DebugManager.Instance.DrawLine(this.Parent.Position, moveVector, 200, Color.Yellow, 1);

			this.MoveVector = moveVector;

			if (this.MoveVector == Vector2.Zero)
			{
				stuckCount += 1;
			}
			else
			{
				stuckCount = 0;
			}

			if (stuckCount >= 5 || (stuckCount >= 1 && this.IsStuck))
			{
				this.IsStuck = true;

				Sprite stuckSprite = new Sprite();

				float randX = Random.RandomFloat(this.Parent.Position.X - 64, this.Parent.Position.X + 64);
				float randY = Random.RandomFloat(this.Parent.Position.Y - 64, this.Parent.Position.Y + 64);

				Vector2 position = new Vector2(randX, randY);

				stuckSprite.Position = position;

				this._stuckTarget = stuckSprite;

				stuckCount = 0;
			}
		}

		private float[] GenerateFinalVectors()
		{
			float[] vectors = new float[this._numberOfDirections];

			for (int i = 0; i < this._numberOfDirections; i++)
			{
				vectors[i] = this._interestVectors[i] - this._dangerVectors[i];

				vectors[i] = MathHelper.Clamp(vectors[i], -0f, 1f);

				if (vectors[i] > 0f)
					DebugManager.Instance.DrawLine(this.Parent.Position, this._directions[i], vectors[i] * this._lookAheadRange, Color.Green, 1);
			}

			return vectors;
		}

		private float[] GenerateDangerVectors()
		{
			float dangerStrength = 10;

			float[] dangers = new float[this._numberOfDirections];

			for (int i = 0; i < this._numberOfDirections; i++)
			{
				RayHit hit = Raycasts.Instance.ShootRay(this.Parent.Position, this._directions[i], this._lookAheadRange, ignore: this.Parent);


				if (hit == null)
					continue;

                if ((!this._avoidTags.Contains(hit.Collider.Parent.Tag) && !this._preferAvoidTags.Contains(hit.Collider.Parent.Tag)) || hit.Collider.Parent == this.Parent)
					continue;

				DebugManager.Instance.DrawLine(this.Parent.Position, this._directions[i], this._lookAheadRange, Color.Red, 1);

				dangers[i] = 10;

				if (i + 1 <= dangers.Length - 1)
					dangers[i + 1] = dangerStrength;
				else if (i + 1 > dangers.Length)
					dangers[0] = dangerStrength;

				if (i - 1 >= 0)
					dangers[i - 1] = dangerStrength;
				else if (i - 1 < 0)
					dangers[dangers.Length - 1] = dangerStrength;

			}

			return dangers;
		}

		private float[] GenerateIntrestVectors()
		{
			float[] interests = new float[this._numberOfDirections];

			for (int i = 0; i < this._numberOfDirections; i++)
			{
				interests[i] = Vector2.Dot(this._directionToTarget, _directions[i]);
			}

			return interests;
		}

		public Vector2[] GetVectorDirections(int numberOfVectors)
		{
			Vector2[] directions = new Vector2[numberOfVectors];

			float angleStep = MathHelper.ToRadians(360) / numberOfVectors;

			for (int i = 0; i < numberOfVectors; i++)
			{
				float angle = angleStep * i;

				directions[i] = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
			}

			return directions;
		}
	}
}
