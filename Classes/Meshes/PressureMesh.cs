namespace CellularAutomaton.Classes.Meshes;

using CellularAutomaton.Classes.Blocks;
using SFML.Graphics;
using SFML.System;

public class PressureMesh : Mesh<Vector2f>
{
    public PressureMesh(Vector2i coord)
        : base(Chunk.Size, coord)
    {
        this[new Vector2i(0, 0)] = new Vector2f(0, 0);
    }

    public override void Update(Scene scene)
    {
        var tempPressureMap = new Vector2f[this.Width, this.Height];

        for (int x = 0; x < this.Width; x++)
        {
            for (int y = 0; y < this.Height; y++)
            {
                tempPressureMap[x, y] = PressureMesh.AverageVel(scene, this.Coord + new Vector2i(x, y));
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
                    new Vector2f((x * Block.Size) + (Block.Size / 2), (y * Block.Size) + (Block.Size / 2)));
                var b = new Vertex(a.Position + (this.Grid[x, y] * 40));

                target.Draw(new Vertex[] { a, b }, PrimitiveType.Lines);
            }
        }
    }

    private static Vector2f AverageVel(Scene scene, Vector2i coord)
    {
        var pressure = new Vector2f(0, 0);
        var count = 0;

        foreach (var delta in Scene.Neighborhood)
        {
            var neighbour = scene.ChunkMesh[coord + delta]?.PressureMesh[coord + delta];
            if (neighbour.HasValue)
            {
                pressure += neighbour.Value;
                count++;
            }
        }

        return pressure / count;
    }
}