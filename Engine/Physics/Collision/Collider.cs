using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Bean.Debug;
using System.Net.Mail;

namespace Bean.PhysicsSystem
{
	public class Collider : Addon
	{
		public List<Collider> CheckedColliders;

		private Color _drawColour;

		public ColliderEdges Edges;

		private PhysicsObject _physicsObject;

		[DebugServerVariable]
		public int Height { get; set; }

		[DebugServerVariable]
		public bool IsRaycast;

		[DebugServerVariable]
		public bool IsSolid = true;

		public EventHandler<CollisionEventArgs> OnCollisionStay;
		public EventHandler<CollisionEventArgs> OnCollisionEnter;
		public EventHandler<CollisionEventArgs> OnCollisionExit;

		private List<Collider> _lastColliders = new List<Collider>();
		private List<Collider> _currentColliders = new List<Collider>();


		[DebugServerVariable]
		public Vector2 PositionOffset;

		public Rectangle Rectangle
		{
			get
			{
				if (Parent is Sprite sprite)
					return new Rectangle((int)(sprite.GetSpriteRectangle().X + PositionOffset.X), (int)(sprite.GetSpriteRectangle().Y + PositionOffset.Y), Width, Height);

				return new Rectangle((int)(Parent.Position.X + PositionOffset.X), (int)(Parent.Position.Y + PositionOffset.Y), Width, Height);
			}
		}

		public bool DoesParentHavePhysicsObject;

		[DebugServerVariable]
		public int Width { get; set; }

		internal List<CollisonGridSquare> GridSquares;

		public string[] IgnoreTags = new string[0];

		public Collider()
		{
			this.CheckedColliders = new List<Collider>();
		}

		public override void Start()
		{
			base.Start();

			if (!IsRaycast)
				Physics.Instance.AddGameCollider(this);

			this.Edges = new ColliderEdges(this);

			this.IsActive = true;

			this._drawColour = new Color(Random.RandomInt(1, 255), Random.RandomInt(1, 255), Random.RandomInt(1, 255), 75);

			this._physicsObject = Parent.GetAddon<PhysicsObject>();

			if (this._physicsObject != null)
			{
				this.DoesParentHavePhysicsObject = true;
			}

		}

		public override void Update()
		{
			base.Update();

			this.CheckedColliders.Clear();

			this.Edges = new ColliderEdges(this);
		}

		public bool CheckCollision(Collider collider)
		{

			if (collider == null)
			{
				return false;
			}

			if (this.IgnoreTags.Contains(collider.Parent.Tag))
			{
				return false;	
			}

			if (this.CheckedColliders.Contains(collider) || collider.CheckedColliders.Contains(this))
			{
				return false;
			}

			if (collider.Parent == this.Parent)
			{
				return false;
			}

			if (this.IsRaycast && collider.IsRaycast)
			{
				return false;
			}

			bool collided = false;

			Vector2 direction = Vector2.Zero;

			Vector2 velocity = (this.DoesParentHavePhysicsObject) ? this._physicsObject.Velocity * Time.Instance.TargetMultiplier : Vector2.Zero;

			Rectangle thisRectangle = this.Rectangle;

			Rectangle colliderRectangle = collider.Rectangle;

			if (thisRectangle.Right + velocity.X > colliderRectangle.Left &&
				thisRectangle.Left < colliderRectangle.Left &&
				thisRectangle.Bottom > colliderRectangle.Top &&
				thisRectangle.Top < colliderRectangle.Bottom)
			{
				collided = true;

				direction.X = -1;
			}
			else if (thisRectangle.Left + velocity.X < colliderRectangle.Right &&
					   thisRectangle.Right > colliderRectangle.Right &&
					   thisRectangle.Bottom > colliderRectangle.Top &&
					   thisRectangle.Top < colliderRectangle.Bottom)
			{
				collided = true;

				direction.X = 1;
			}


			if (thisRectangle.Bottom + velocity.Y > colliderRectangle.Top &&
				 thisRectangle.Top < colliderRectangle.Top &&
				 thisRectangle.Right > colliderRectangle.Left &&
				 thisRectangle.Left < colliderRectangle.Right)
			{
				collided = true;

				direction.Y = -1;
			}
			if (thisRectangle.Top + velocity.Y < colliderRectangle.Bottom &&
				 thisRectangle.Bottom > colliderRectangle.Bottom &&
				 thisRectangle.Right > colliderRectangle.Left &&
				 thisRectangle.Left < colliderRectangle.Right)
			{
				collided = true;

				direction.Y = 1;
			}

			if (!collided && (colliderRectangle.Contains(thisRectangle) || thisRectangle.Contains(colliderRectangle)))
			{
				collided = true;
				direction = Vector2.Zero;
			}


			if (collided)
			{
				if (!collider.IsRaycast)
				{
					this.OnCollide(collider, direction);
					collider.OnCollide(this, -direction);
				}

				return true;
			}

			return false;
		}


		public void DrawCollider(SpriteBatch spriteBatch, Texture2D texture)
		{
			spriteBatch.Draw(texture, new Vector2(this.Rectangle.X, this.Rectangle.Y) - base.Parent.Scene.Camera.Position, null, this._drawColour, 0, Vector2.Zero, new Vector2(this.Rectangle.Width, this.Rectangle.Height), SpriteEffects.None, 0.9f);
		}

		public override void RemoveFromGame()
		{
			base.RemoveFromGame();

			Physics.Instance.RemoveGameCollider(this);
		}

		public void BeginCollisonCheck()
		{
			this._lastColliders = new List<Collider>(this._currentColliders);

			this._currentColliders.Clear();
		}

		protected virtual void OnCollide(Collider collider, Vector2 direction)
		{
			if (!this._currentColliders.Contains(collider))
			{
				this._currentColliders.Add(collider);
			}
			else
			{
				return;
			}

			if (this._currentColliders.Contains(collider) && this._lastColliders.Contains(collider))
			{
				this.OnCollisionStay?.Invoke(this, new CollisionEventArgs(collider, direction));
			}
			else if (this._currentColliders.Contains(collider) && !this._lastColliders.Contains(collider))
			{
				this.OnCollisionEnter?.Invoke(this, new CollisionEventArgs(collider, direction));
			}

		}

		public override void LateUpdate()
		{
			base.LateUpdate();

			foreach (Collider collider in this._lastColliders)
			{
				if (!this._currentColliders.Contains(collider))
					this.OnCollisionExit?.Invoke(this, new CollisionEventArgs(collider, Vector2.Zero));

			}
        }

    }

    public class CollisionEventArgs : EventArgs
    {
		public Collider Collider;

		public Vector2 Direction;

		public CollisionEventArgs(Collider collider, Vector2 direction) : base()
		{
			this.Collider = collider;

			this.Direction = direction;
		}
    }
}
