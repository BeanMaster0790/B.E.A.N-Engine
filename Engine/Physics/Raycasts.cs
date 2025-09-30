using Bean.Graphics;
using Bean.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Bean.PhysicsSystem
{
    public class Raycasts
    {
        public static Raycasts Instance = new Raycasts();

        public Collider OverlapBox(Vector2 position, int size)
        {
            WorldProp tempSprite = new WorldProp() { Name = "TempCollider", Position = position, IsTemp = true };

			Collider tempCollider = new Collider()
            {
                Width = size,
                Height = size,
                IsRaycast = true,
                PositionOffset = -new Vector2(size / 2, size / 2)
			};

            tempSprite.AddAddon(tempCollider);

            Rectangle rayBounds = tempCollider.Rectangle;
            if (rayBounds.Width < 0) { rayBounds.X += rayBounds.Width; rayBounds.Width = -rayBounds.Width; }
            if (rayBounds.Height < 0) { rayBounds.Y += rayBounds.Height; rayBounds.Height = -rayBounds.Height; }

            List<CollisonGridSquare> relevantSquares = Physics.Instance.GetGridSquaresIntersecting(rayBounds);

            foreach (CollisonGridSquare gridSquare in relevantSquares)
            {
                foreach (Collider collider in gridSquare.GridColliders)
                {
                    if (collider.CheckCollision(tempCollider))
                    {
                        return collider;    
                    }
                }    
            }

            return null;
        }

        public Collider[] OverlapBoxAll(Vector2 position, int size)
        {
            WorldProp tempSprite = new WorldProp() { Name = "TempCollider", IsTemp = true , Position = position};

			Collider tempCollider = new Collider()
            {
                Width = size,
                Height = size,
                IsRaycast = true,
                PositionOffset = -new Vector2(size / 2, size / 2)
			};

			tempSprite.AddAddon(tempCollider);

            List<Collider> results = new List<Collider>();

            Rectangle rayBounds = tempCollider.Rectangle;
            if (rayBounds.Width < 0) { rayBounds.X += rayBounds.Width; rayBounds.Width = -rayBounds.Width; }
            if (rayBounds.Height < 0) { rayBounds.Y += rayBounds.Height; rayBounds.Height = -rayBounds.Height; }

            List<CollisonGridSquare> relevantSquares = Physics.Instance.GetGridSquaresIntersecting(rayBounds);

            foreach (CollisonGridSquare gridSquare in relevantSquares)
            {
                foreach (Collider collider in gridSquare.GridColliders)
                {
                    if (collider.CheckCollision(tempCollider))
                    {
                        results.Add(collider);   
                    }
                }    
            }
            return results.ToArray();
        }

        public RayHit ShootRay(Vector2 position, Vector2 direction, int range, Prop ignore = null, string[] ignoreTags = null)
        {
            Line ray = new Line(position, position + direction * range);

            RayHit ClosestHit = new RayHit(new Vector2(range * 4, range * 4), null);

            int checks = 0;
            
            Rectangle rayBounds = new Rectangle((int)position.X, (int)position.Y, (int)(direction.X * range), (int)(direction.Y * range));
            if (rayBounds.Width < 0) { rayBounds.X += rayBounds.Width; rayBounds.Width = -rayBounds.Width; }
            if (rayBounds.Height < 0) { rayBounds.Y += rayBounds.Height; rayBounds.Height = -rayBounds.Height; }

            List<CollisonGridSquare> relevantSquares = Physics.Instance.GetGridSquaresIntersecting(rayBounds);

            foreach (CollisonGridSquare gridSquare in relevantSquares)
            {
                foreach (Collider collider in gridSquare.GridColliders)
                {
                    if (ignore == collider.Parent || !collider.IsActive || (ignoreTags != null && ignoreTags.Contains(collider.Parent.Tag)))
                        continue;

                    foreach (Line edge in collider.Edges.Edges)
                    {
                        Vector2? intercept = GetIntercept(ray, edge);

                        checks++;

                        if (intercept != null)
                        {
                            float distanceFromOrigin = Vector2.Distance(position, (Vector2)intercept);


                            if (distanceFromOrigin > range)
                                continue;

                            if (distanceFromOrigin < Vector2.Distance(position, ClosestHit.HitPoint))
                                ClosestHit = new RayHit((Vector2)intercept, collider);

                        }
                    }
                }
            }

            if (ClosestHit.Collider == null)
            return null;

            return ClosestHit;
        }

		public RayHit[] ShootRayAll(Vector2 position, Vector2 direction, int range, Prop ignore = null, string[] ignoreTags = null)
		{
			Line ray = new Line(position, position + direction * range);

            List<RayHit> results = new List<RayHit>();

            int checks = 0;
            
            Rectangle rayBounds = new Rectangle((int)position.X, (int)position.Y, (int)(direction.X * range), (int)(direction.Y * range));
            if (rayBounds.Width < 0) { rayBounds.X += rayBounds.Width; rayBounds.Width = -rayBounds.Width; }
            if (rayBounds.Height < 0) { rayBounds.Y += rayBounds.Height; rayBounds.Height = -rayBounds.Height; }

            List<CollisonGridSquare> relevantSquares = Physics.Instance.GetGridSquaresIntersecting(rayBounds);

            foreach (CollisonGridSquare gridSquare in relevantSquares)
            {
                foreach (Collider collider in gridSquare.GridColliders)
                {
                    if (ignore == collider.Parent || !collider.IsActive || (ignoreTags != null && ignoreTags.Contains(collider.Tag)))
                        continue;

                    foreach (Line edge in collider.Edges.Edges)
                    {
                        Vector2? intercept = GetIntercept(ray, edge);

                        checks++;

                        if (intercept != null)
                        {
                            float distanceFromOrigin = Vector2.Distance(position, (Vector2)intercept);


                            if (distanceFromOrigin > range)
                                continue;

                            results.Add(new RayHit((Vector2)intercept, collider));

                        }
                    }
                }
            }

			return results.ToArray();

		}

		public Vector2? GetIntercept(Line ray, Line edge)
        {
			float denominator = (ray.StartPoint.X - ray.EndPoint.X) * (edge.StartPoint.Y - edge.EndPoint.Y) - (ray.StartPoint.Y - ray.EndPoint.Y) * (edge.StartPoint.X - edge.EndPoint.X);

			if (denominator == 0)
				return null;

			float t = ((ray.StartPoint.X - edge.StartPoint.X) * (edge.StartPoint.Y - edge.EndPoint.Y) - (ray.StartPoint.Y - edge.StartPoint.Y) * (edge.StartPoint.X - edge.EndPoint.X)) / denominator;
			float u = -((ray.StartPoint.X - ray.EndPoint.X) * (ray.StartPoint.Y - edge.StartPoint.Y) - (ray.StartPoint.Y - ray.EndPoint.Y) * (ray.StartPoint.X - edge.StartPoint.X)) / denominator;

			if (t >= 0 && u >= 0 && u <= 1)
			{
				float intersectionX = ray.StartPoint.X + t * (ray.EndPoint.X - ray.StartPoint.X);
				float intersectionY = ray.StartPoint.Y + t * (ray.EndPoint.Y - ray.StartPoint.Y);
				return new Vector2(intersectionX, intersectionY);
			}

			return null;
		}
    }

    public class RayHit
    {
        public Vector2 HitPoint;

        public Collider Collider;

        public RayHit(Vector2 hitPoint, Collider collider)
        {
            this.HitPoint = hitPoint;
            this.Collider = collider;
        }
    }

    public class ColliderEdges
    {
        public Line Top;
        public Line Bottom;
        public Line Left;
        public Line Right;

        public Line[] Edges = new Line[4];

        public ColliderEdges(Collider collider)
        {
			Top = new Line(new Vector2(collider.Rectangle.Left, collider.Rectangle.Top), new Vector2(collider.Rectangle.Right, collider.Rectangle.Top));
			Bottom = new Line(new Vector2(collider.Rectangle.Left, collider.Rectangle.Bottom), new Vector2(collider.Rectangle.Right, collider.Rectangle.Bottom));
			Left = new Line(new Vector2(collider.Rectangle.Left, collider.Rectangle.Top), new Vector2(collider.Rectangle.Left, collider.Rectangle.Bottom));
			Right = new Line(new Vector2(collider.Rectangle.Right, collider.Rectangle.Top), new Vector2(collider.Rectangle.Right, collider.Rectangle.Bottom));

			this.Edges[0] = this.Top;
            this.Edges[1] = this.Bottom;
            this.Edges[2] = this.Left;
            this.Edges[3] = this.Right;
        }
    }
}
