﻿namespace CellularAutomaton.Classes
{
    using CellularAutomaton.Classes.Blocks;
    using CellularAutomaton.Classes.Walls;
    using CellularAutomaton.Interfaces;
    using SFML.Graphics;
    using SFML.System;

    public class Chunk
    {
        public Chunk(int x, int y)
        {
            this.Coord = new Vector2i(x, y);

            this.InitMap();
        }

        public static Vector2i Size = new (20, 20);

        public Vector2i Coord { get; set; }

        public IBlock[,] Map { get; set; } = new IBlock[Chunk.Size.X, Chunk.Size.Y];

        public void Draw(RenderWindow window)
        {
            var border = new RectangleShape((Vector2f)Chunk.Size * IBlock.Size)
            {
                FillColor = Color.Transparent,
                OutlineColor = Color.Red,
                OutlineThickness = 2,
                Position = (Vector2f)this.Coord * IBlock.Size,
            };
            window.Draw(border);

            var view = window.GetView();
            var viewRect = new FloatRect(view.Center - (view.Size / 2), view.Size);
            foreach (var block in this.Map)
            {
                if (viewRect.Intersects(block.CollisionBox.GetGlobalBounds()))
                {
                    block.Draw(window);
                }
            }
        }

        public void Update(Scene scene)
        {
            var newMap = (IBlock[,])this.Map.Clone();

            foreach (var block in newMap)
            {
                block.Update(scene);
            }
        }

        public bool IsValidCoords(Vector2i coords)
        {
            var localCoord = coords - this.Coord;
            return localCoord.X >= 0 &&
                localCoord.X < this.Map.GetLength(0) &&
                localCoord.Y >= 0 &&
                localCoord.Y < this.Map.GetLength(1);
        }

        public IBlock[] GetAllBlocks()
        {
            var blocks = new List<IBlock>();

            foreach (var block in this.Map)
            {
                blocks.Add(block);
            }

            return blocks.ToArray();
        }

        public IBlock GetBlock(Vector2i coords)
        {
            if (this.IsValidCoords(coords))
            {
                return this.Map[coords.X - this.Coord.X, coords.Y - this.Coord.Y];
            }

            return null;
        }

        public IBlock GetBlock(int x, int y)
            => this.GetBlock(new Vector2i(x, y));

        public void SetBlock(IBlock block, Vector2i coords)
        {
            if (!this.IsValidCoords(coords))
            {
                return;
            }

            var oldBlock = this.GetBlock(coords);
            if (oldBlock is not null)
            {
                if (block.Wall is null)
                {
                    block.Wall = oldBlock.Wall.Copy();
                }

                oldBlock.Dispose();
            }

            block.Coords = coords;
            block.CollisionBox.Position = new Vector2f(coords.X * IBlock.Size, coords.Y * IBlock.Size);
            block.Wall.CollisionBox.Position = block.CollisionBox.Position;
            this.Map[coords.X - this.Coord.X, coords.Y - this.Coord.Y] = block;
        }

        public void SetBlock(IBlock block, int x, int y)
            => this.SetBlock(block, new Vector2i(x, y));

        public void Dispose()
        {
            foreach (var block in this.Map)
            {
                block.Dispose();
            }
        }

        private void InitMap()
        {
            for (int x = 0; x < Chunk.Size.X; x++)
            {
                for (int y = 0; y < Chunk.Size.Y; y++)
                {
                    this.SetBlock(new Empty() { Wall = new EmptyWall() }, this.Coord.X + x, this.Coord.Y + y);
                }
            }
        }
    }
}
