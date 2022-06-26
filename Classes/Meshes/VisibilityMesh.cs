namespace CellularAutomaton.Classes.Meshes;

using CellularAutomaton.Classes.Blocks;
using SFML.Graphics;
using SFML.System;

public class VisibilityMesh : Mesh<bool, Chunk>
{
    public VisibilityMesh(Chunk chunk, Vector2i coord)
        : base(chunk, Chunk.Size, coord)
    {
    }

    public override void FastUpdate()
    {
        var blockSize = new Vector2f(Block.Size, Block.Size);
        var viewRect = new FloatRect(
            this.Parent.Scene.Camera.Center - (this.Parent.Scene.Camera.Size / 2) - blockSize,
            this.Parent.Scene.Camera.Size + (blockSize * 2));

        for (int x = 0; x < this.Width; x++)
        {
            for (int y = 0; y < this.Height; y++)
            {
                this.Grid[x, y] = false;
            }
        }

        var chunkRect = new FloatRect((Vector2f)this.Coord * Block.Size, (Vector2f)Chunk.Size * Block.Size);
        if (viewRect.Intersects(chunkRect))
        {
            var xLower = Math.Max(0, (int)(viewRect.Left / Block.Size) - this.Coord.X);
            var yLower = Math.Max(0, (int)(viewRect.Top / Block.Size) - this.Coord.Y);

            var xUpper = Math.Min((int)((viewRect.Left + viewRect.Width) / Block.Size) - this.Coord.X, this.Width);
            var yUpper = Math.Min((int)((viewRect.Top + viewRect.Height) / Block.Size) - this.Coord.Y, this.Height);

            for (int x = xLower; x < xUpper; x++)
            {
                for (int y = yLower; y < yUpper; y++)
                {
                    this.Grid[x, y] = true;
                }
            }
        }
    }
}
