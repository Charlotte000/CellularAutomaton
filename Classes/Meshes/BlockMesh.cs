namespace CellularAutomaton.Classes.Meshes;

using CellularAutomaton.Classes.Blocks;
using SFML.Graphics;
using SFML.System;

public class BlockMesh : Mesh<Block, Chunk>
{
    public BlockMesh(Chunk chunk, Vector2i coord)
        : base(chunk, Chunk.Size, coord)
    {
        for (int x = 0; x < this.Width; x++)
        {
            for (int y = 0; y < this.Height; y++)
            {
                this[x + this.Coord.X, y + this.Coord.Y] = new Empty();
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
                    value!.Light = oldBlock.Light;
                    oldBlock.OnDelete();
                }

                value!.Coords = new Vector2i(x, y);
                value.CollisionBox.Position = (Vector2f)value.Coords * Block.Size;
                value.Chunk = this.Parent;
                this.Grid[x - this.Coord.X, y - this.Coord.Y] = value!;
            }
        }
    }

    public override void Draw(RenderTarget target, RenderStates states)
    {
        foreach (var block in this.Grid)
        {
            if (block.IsVisible)
            {
                var blockRenderState = new RenderStates(states);
                blockRenderState.Transform.Translate(block.CollisionBox.Position);
                target.Draw(block, blockRenderState);
            }
        }
    }

    public override void DrawMesh(RenderTarget target)
    {
        var border = new RectangleShape((Vector2f)Chunk.Size * Block.Size)
        {
            FillColor = Color.Transparent,
            OutlineColor = Color.Red,
            OutlineThickness = 2,
            Position = (Vector2f)this.Coord * Block.Size,
        };
        target.Draw(border);
    }

    public override void Update()
    {
        foreach (var block in this.Grid)
        {
            block.WasUpdated = false;
        }

        foreach (var block in this.Grid)
        {
            if (!block.WasUpdated)
            {
                block.WasUpdated = true;
                block.OnUpdate();
            }
        }
    }

    public override void Dispose()
    {
        foreach (var block in this.Grid)
        {
            block.OnDelete();
        }
    }
}
