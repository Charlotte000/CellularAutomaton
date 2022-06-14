namespace CellularAutomaton.Classes.Mesh;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Walls;
using SFML.Graphics;
using SFML.System;

public class BlockMesh : Mesh<Block>
{
    public BlockMesh(Vector2i coord)
        : base(Chunk.Size, coord)
    {
        for (int x = 0; x < this.Width; x++)
        {
            for (int y = 0; y < this.Height; y++)
            {
                this[x + this.Coord.X, y + this.Coord.Y] = new Empty() { Wall = new EmptyWall() };
            }
        }
    }

    public override Block? this[int x, int y]
    {
        set
        {
            if (this.IsValidCoord(x, y))
            {
                var oldBlock = this[x, y];
                if (oldBlock is not null)
                {
                    if (value!.Wall is null)
                    {
                        value!.Wall = oldBlock.Wall?.Copy();
                    }

                    value!.Light = oldBlock.Light;
                    oldBlock.OnDelete();
                }

                value!.Coords = new Vector2i(x, y);
                value.CollisionBox.Position = (Vector2f)value.Coords * Block.Size;
                this.Grid[x - this.Coord.X, y - this.Coord.Y] = value!;
            }
        }
    }

    public void Draw(RenderTarget target, RenderStates states)
    {
        var border = new RectangleShape((Vector2f)Chunk.Size * Block.Size)
        {
            FillColor = Color.Transparent,
            OutlineColor = Color.Red,
            OutlineThickness = 2,
            Position = (Vector2f)this.Coord * Block.Size,
        };
        target.Draw(border, states);
    }
}
