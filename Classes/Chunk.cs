namespace CellularAutomaton.Classes;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Walls;
using SFML.Graphics;
using SFML.System;

public class Chunk : Drawable
{
    public Chunk(int x, int y)
    {
        this.Coord = new Vector2i(x, y);

        this.InitMap();
    }

    public static Vector2i Size { get; } = new (20, 20);

    public Vector2i Coord { get; set; }

    public Block[,] Map { get; set; } = new Block[Chunk.Size.X, Chunk.Size.Y];

    public Vector2f[,] VelMap { get; set; } = new Vector2f[Chunk.Size.X, Chunk.Size.Y];

    public void Draw(RenderTarget target, RenderStates states)
    {
        // this.DrawBorder(target, states);
        foreach (var block in this.Map)
        {
            if (block.IsVisible)
            {
                var blockRenderState = new RenderStates(states);
                blockRenderState.Transform.Translate(block.CollisionBox.Position);
                target.Draw(block, blockRenderState);
            }
        }

        // this.DrawVelMap(target, states);
    }

    public void Update(Scene scene)
    {
        foreach (var block in this.Map)
        {
            block.WasUpdated = false;
        }

        foreach (var block in this.Map)
        {
            if (!block.WasUpdated)
            {
                block.WasUpdated = true;
                block.OnUpdate(scene);
            }
        }

        this.UpdateVelMap(scene);
    }

    public bool IsValidCoords(Vector2i coords)
    {
        var localCoord = coords - this.Coord;
        return localCoord.X >= 0 &&
            localCoord.X < this.Map.GetLength(0) &&
            localCoord.Y >= 0 &&
            localCoord.Y < this.Map.GetLength(1);
    }

    public Block[] GetAllBlocks()
    {
        var blocks = new List<Block>();

        foreach (var block in this.Map)
        {
            blocks.Add(block);
        }

        return blocks.ToArray();
    }

    public Block? GetBlock(Vector2i coords)
    {
        if (!this.IsValidCoords(coords))
        {
            return null;
        }

        return this.Map[coords.X - this.Coord.X, coords.Y - this.Coord.Y];
    }

    public Block? GetBlock(int x, int y)
        => this.GetBlock(new Vector2i(x, y));

    public void SetBlock(Block block, Vector2i coords)
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
                block.Wall = oldBlock.Wall?.Copy();
            }

            block.Light = oldBlock.Light;
            oldBlock.OnDelete();
        }

        this.SetBlockForce(block, coords.X, coords.Y);
    }

    public void SetBlock(Block block, int x, int y)
        => this.SetBlock(block, new Vector2i(x, y));

    public Vector2f? GetVel(Vector2i coords)
    {
        if (!this.IsValidCoords(coords))
        {
            return null;
        }

        return this.VelMap[coords.X - this.Coord.X, coords.Y - this.Coord.Y];
    }

    public Vector2f? GetVel(int x, int y)
        => this.GetVel(new Vector2i(x, y));

    public void AddVel(Vector2f vel, Vector2i coords)
    {
        if (!this.IsValidCoords(coords))
        {
            return;
        }

        this.VelMap[coords.X - this.Coord.X, coords.Y - this.Coord.Y] += vel;
    }

    public void AddVel(Vector2f vel, int x, int y)
        => this.AddVel(vel, new Vector2i(x, y));

    public void Dispose()
    {
        foreach (var block in this.Map)
        {
            block.OnDelete();
        }
    }

    private void DrawVelMap(RenderTarget target, RenderStates states)
    {
        for (int x = 0; x < this.VelMap.GetLength(0); x++)
        {
            for (int y = 0; y < this.VelMap.GetLength(1); y++)
            {
                var a = new Vertex(
                    (Vector2f)(this.Coord * Block.Size) + new Vector2f(x * Block.Size, y * Block.Size));
                var b = new Vertex(a.Position + (this.VelMap[x, y] * 40));
                target.Draw(new Vertex[] { a, b }, PrimitiveType.Lines);
            }
        }
    }

    private void DrawBorder(RenderTarget target, RenderStates states)
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

    private void SetBlockForce(Block block, Vector2i coords)
    {
        block.Coords = coords;
        block.CollisionBox.Position = (Vector2f)coords * Block.Size;
        this.Map[coords.X - this.Coord.X, coords.Y - this.Coord.Y] = block;
    }

    private void SetBlockForce(Block block, int x, int y)
        => this.SetBlockForce(block, new Vector2i(x, y));

    private void InitMap()
    {
        for (int x = 0; x < Chunk.Size.X; x++)
        {
            for (int y = 0; y < Chunk.Size.Y; y++)
            {
                this.SetBlockForce(new Empty() { Wall = new EmptyWall() }, this.Coord.X + x, this.Coord.Y + y);
            }
        }
    }

    private void UpdateVelMap(Scene scene)
    {
        var tempVelMap = new Vector2f[this.VelMap.GetLength(0), this.VelMap.GetLength(1)];

        for (int x = 0; x < this.VelMap.GetLength(0); x++)
        {
            for (int y = 0; y < this.VelMap.GetLength(1); y++)
            {
                tempVelMap[x, y] = this.AverageVel(scene, this.Coord + new Vector2i(x, y));
            }
        }

        this.VelMap = tempVelMap;
    }

    private Vector2f AverageVel(Scene scene, Vector2i coord)
    {
        var vel = new Vector2f(0, 0);
        var count = 0;

        foreach (var delta in Scene.Neighborhood)
        {
            var neighbour = scene.GetVel(coord + delta);
            if (neighbour.HasValue)
            {
                vel += neighbour.Value;
                count++;
            }
        }

        return vel / count;
    }
}
