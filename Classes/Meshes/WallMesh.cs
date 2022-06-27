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
                this[x + this.Coord.X, y + this.Coord.Y] = new EmptyWall();
            }
        }
    }

    public override Wall? this[int x, int y]
    {
        set
        {
            if (this.IsValidCoord(x, y))
            {
                this[x, y]?.OnDestroy();

                value!.Coord = new Vector2i(x, y);
                value.CollisionBox.Position = (Vector2f)value.Coord * Block.Size;
                value.Chunk = this.Parent;
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
                        wallRenderState.Transform.Translate((Vector2f)this.Grid[x, y].Coord * Block.Size);
                        target.Draw(this.Grid[x, y], wallRenderState);
                    }
                }
            }
        }
    }
}
