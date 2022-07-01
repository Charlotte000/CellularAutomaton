namespace CellularAutomaton.Classes.Meshes;

using CellularAutomaton.Classes.Blocks;
using SFML.Graphics;
using SFML.System;

public class PressureMesh : Mesh<Vector2f, Chunk>
{
    public PressureMesh(Chunk chunk, Vector2i coord)
        : base(chunk, Chunk.Size, coord)
    {
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        var tempPressureMap = new Vector2f[this.Width, this.Height];

        for (int x = 0; x < this.Width; x++)
        {
            for (int y = 0; y < this.Height; y++)
            {
                tempPressureMap[x, y] = this.AverageVel(this.Coord + new Vector2i(x, y));
            }
        }

        this.Grid = tempPressureMap;
    }

    public override void DrawMesh(RenderTarget target)
    {
        for (int x = 0; x < this.Width; x++)
        {
            for (int y = 0; y < this.Height; y++)
            {
                var a = new Vertex(
                    (Vector2f)(this.Coord * Block.Size) +
                    (new Vector2f(x, y) * Block.Size) + (new Vector2f(Block.Size, Block.Size) / 2));
                var b = new Vertex(a.Position + (this.Grid[x, y] * 40));

                target.Draw(new Vertex[] { a, b }, PrimitiveType.Lines);
            }
        }
    }

    private Vector2f AverageVel(Vector2i coord)
    {
        var pressure = new Vector2f(0, 0);
        var count = 0;

        foreach (var delta in Scene.Neighborhood)
        {
            var neighbour = this.Parent.Scene.ChunkMesh[coord + delta]?.PressureMesh[coord + delta];
            if (neighbour.HasValue)
            {
                pressure += neighbour.Value;
                count++;
            }
        }

        return pressure / count;
    }
}