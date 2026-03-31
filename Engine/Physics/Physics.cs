using Bean.Debug;
using Bean.Graphics;
using Bean.Player;
using Bean.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Bean.PhysicsSystem
{
    public class Physics
    {
        private List<Collider> _gameColliders = new List<Collider>();

        internal List<Collider> MovingColliders = new List<Collider>();

        public static Physics Instance = new Physics();

        internal Texture2D _texture;

        private List<CollisonGridSquare> _grid;
        private Dictionary<Point, CollisonGridSquare> _gridMap = new Dictionary<Point, CollisonGridSquare>();


        private int _gridSplitValue = 25;


        public void AddGameCollider(Collider collider)
        {
            this._gameColliders.Add(collider);

            UpdateGridFromCollider(collider);
        }


        private void RemoveColliderfromGrid(Collider collider)
        {
            if (collider.IsRaycast)
                return;

            foreach (CollisonGridSquare gridSquare in collider.GridSquares)
            {
                gridSquare.GridColliders.Remove(collider);
            }

            collider.GridSquares.Clear();
        }

        public List<CollisonGridSquare> GetGridSquaresIntersecting(Rectangle area)
        {
            List<CollisonGridSquare> result = new List<CollisonGridSquare>();

            int minX = area.Left / _gridSplitValue;
            int maxX = area.Right / _gridSplitValue;
            int minY = area.Top / _gridSplitValue;
            int maxY = area.Bottom / _gridSplitValue;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (_gridMap.TryGetValue(new Point(x, y), out var square))
                        result.Add(square);
                }
            }

            return result;
        }

        public void UpdateGridFromCollider(Collider collider)
        {
            if (this._grid == null)
                CreateNewGrid();

            int minGridX = this._grid[0].GridRectangle.Left;
            int minGridY = this._grid[0].GridRectangle.Top;

            int maxGridX = this._grid[this._grid.Count - 1].GridRectangle.Right;
            int maxGridY = this._grid[this._grid.Count - 1].GridRectangle.Bottom;

            int minPosX = collider.ColliderRectangle.Left;
            int minPosY = collider.ColliderRectangle.Top;

            int maxPosX = collider.ColliderRectangle.Right;
            int maxposY = collider.ColliderRectangle.Bottom;

            if (minPosX < minGridX || minPosY < minGridY || maxPosX > maxGridX || maxposY > maxGridY)
            {
                CreateNewGrid();
            }

            SortSingleColliderIntoGrid(collider);
        }

        private void CreateNewGrid()
        {
            List<CollisonGridSquare> grid = new List<CollisonGridSquare>();

            int minPosX = int.MaxValue;
            int maxPosX = int.MinValue;

            int minPosY = int.MaxValue;
            int maxPosY = int.MinValue;

            foreach (Collider collider in this._gameColliders)
            {
                Rectangle colliderRect = collider.ColliderRectangle;

                if (colliderRect.Left < minPosX)
                {
                    minPosX = colliderRect.Left;
                }

                if (colliderRect.Right > maxPosX)
                {
                    maxPosX = colliderRect.Right;
                }

                if (colliderRect.Top < minPosY)
                {
                    minPosY = colliderRect.Top;
                }

                if (colliderRect.Bottom > maxPosY)
                {
                    maxPosY = colliderRect.Bottom;
                }

            }

            int minGridX = (int)(MathF.Floor((float)minPosX / _gridSplitValue) * _gridSplitValue);
            int maxGridX = (int)(MathF.Ceiling((float)maxPosX / _gridSplitValue) * _gridSplitValue);
            int minGridY = (int)(MathF.Floor((float)minPosY / _gridSplitValue) * _gridSplitValue);
            int maxGridY = (int)(MathF.Ceiling((float)maxPosY / _gridSplitValue) * _gridSplitValue);

            for (int x = minGridX; x <= maxGridX; x += _gridSplitValue)
            {
                for (int y = minGridY; y <= maxGridY; y += _gridSplitValue)
                {
                    CollisonGridSquare gridSquare = new CollisonGridSquare(new Vector2(x, y), _gridSplitValue, _gridSplitValue);

                    if (this._grid != null)
                    {
                        CollisonGridSquare existingGrid = this._grid.FirstOrDefault(o => o.Position == gridSquare.Position);

                        if (existingGrid != null)
                            gridSquare.GridColliders = existingGrid.GridColliders;
                    }

                    grid.Add(gridSquare);

                    Point cell = new Point((int)(x / _gridSplitValue), (int)(y / _gridSplitValue));
                    this._gridMap[cell] = gridSquare;
                }
            }

            this._grid = grid;

        }

        public void SortSingleColliderIntoGrid(Collider collider)
        {
            if (collider.GridSquares == null)
                collider.GridSquares = new List<CollisonGridSquare>();

            List<CollisonGridSquare> newSquares = new List<CollisonGridSquare>();

            Rectangle rect = collider.ColliderRectangle;

            int minX = rect.Left / _gridSplitValue;
            int maxX = rect.Right / _gridSplitValue;
            int minY = rect.Top / _gridSplitValue;
            int maxY = rect.Bottom / _gridSplitValue;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Point key = new Point(x, y);
                    if (_gridMap.TryGetValue(key, out CollisonGridSquare square))
                    {
                        newSquares.Add(square);

                        if (collider.GridSquares.Contains(square))
                            continue;

                        if (collider.DoesParentHavePhysicsObject)
                            square.GridColliders.Insert(0, collider);
                        else if (!collider.IsRaycast)
                            square.GridColliders.Add(collider);

                        collider.GridSquares.Add(square);
                    }
                }
            }

            if (newSquares.Count < collider.GridSquares.Count)
            {
                foreach (CollisonGridSquare square in collider.GridSquares.ToArray())
                {
                    if (!newSquares.Contains(square))
                    {
                        square.GridColliders.Remove(collider);
                        collider.GridSquares.Remove(square);
                    }
                }
            }
        }


        public void DrawColliders(SpriteBatch spriteBatch)
        {
            if (this._texture == null)
            {
                this._texture = new Texture2D(GraphicsManager.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);

                this._texture.SetData(new[] { Color.White });
            }


            foreach (Collider collider in GetGameColliders())
            {
                collider.DrawCollider(spriteBatch, this._texture);
            }

        }

        public Collider[] GetGameColliders()
        {
            return _gameColliders.ToArray();
        }

        public void RemoveGameCollider(Collider collider)
        {
            if (collider.IsRaycast)
                return;

            this._gameColliders.Remove(collider);

            if(this.MovingColliders.Contains(collider))
                this.MovingColliders.Remove(collider);

            RemoveColliderfromGrid(collider);

        }



        public void Update()
        {
            if (this._gameColliders.Count < 2)
                return;

            foreach (Collider movingCol in this.MovingColliders.ToArray())
            {
                UpdateGridFromCollider(movingCol);

                movingCol.BeginCollisonCheck();

                foreach (CollisonGridSquare gridSquare in movingCol.GridSquares.ToArray())
                {
                    Collider[] colliders = gridSquare.GridColliders.ToArray();

                    for (int i = 0; i < colliders.Length; i++)
                    {
                        Collider currentCollider = colliders[i];

                        for (int j = 0; j < colliders.Length; j++)
                        {
                            if (i >= j)
                                continue;

                            Collider collider = colliders[j];
                            if (collider.IsActive && currentCollider.IsActive)
                                currentCollider.CheckCollision(collider);
                        }
                    }
                }

            }

            this.MovingColliders.Clear();
        }

    }
}
