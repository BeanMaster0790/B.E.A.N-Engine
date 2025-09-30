using System.Collections.Generic;
using Bean.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bean.PhysicsSystem
{
    public class CollisonGridSquare
    {
        public List<Collider> GridColliders = new List<Collider>();

        public Vector2 Position;

        public int Width;
        public int Height;

        public Rectangle GridRectangle;

        public CollisonGridSquare(Vector2 position, int width, int height)
        {
            this.Position = position;
            this.Width = width;
            this.Height = height;

            this.GridRectangle = new Rectangle((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height);
        }
    }

}