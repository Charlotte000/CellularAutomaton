namespace CellularAutomaton.Classes.Meshes;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Walls;
using SFML.Graphics;
using SFML.System;

public class WallMesh : Mesh<Wall, Chunk>
{
    public WallMesh(Chunk chunk, Vector2i coord)
        : base(chunk, Chunk.Size, coord)
    {
        for (int x = 0; x < this.Width; x++)
        {
            for (int y = 0; y < this.Height; y++)
            {
                this.Grid[x, y] = new EmptyWall();
            }
        }
    }

    public override Wall? this[int x, int y]
    {
        set
        {
            if (this.IsValidCoord(x, y))
            {
                var oldWall = this[x, y];
                if (oldWall is not null)
                {
                    oldWall.OnDelete();
                }

                value!.Coords = new Vector2i(x, y);
                this.Grid[x - this.Coord.X, y - this.Coord.Y] = value!;
            }
        }
    }

    public override void Draw(RenderTarget target, RenderStates states)
    {
        for (int x = 0; x < this.Width; x++)
        {
            for (int y = 0; y < this.Height; y++)
            {
                if (this.Parent.VisibilityMesh.Grid[x, y])
                {
                    var block = this.Parent.BlockMesh.Grid[x, y];
                    if (block.IsTransparent)
                    {
                        var wallRenderState = new RenderStates(states);
                        wallRenderState.Transform.Translate((Vector2f)this.Grid[x, y].Coords * Block.Size);
                        target.Draw(this.Grid[x, y], wallRenderState);
                    }
                }
            }
        }
    }
}
