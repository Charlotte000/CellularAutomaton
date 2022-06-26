namespace CellularAutomaton.Classes;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Meshes;
using CellularAutomaton.Classes.Walls;
using SFML.Graphics;
using SFML.System;
using System.Collections;

public class Chunk : Drawable, IEnumerable<(Block block, Wall wall)>
{
    public Chunk(int x, int y)
    {
        this.Coord = new Vector2i(x, y);

        this.WallMesh = new (this, this.Coord);
        this.BlockMesh = new (this, this.Coord);
        this.PressureMesh = new (this, this.Coord);
        this.LightMesh = new (this, this.Coord);
        this.VisibilityMesh = new (this, this.Coord);
    }

    public static Vector2i Size { get; } = new (20, 20);

    public Vector2i Coord { get; set; }

    public WallMesh WallMesh { get; set; }

    public BlockMesh BlockMesh { get; set; }

    public PressureMesh PressureMesh { get; set; }

    public LightMesh LightMesh { get; set; }

    public VisibilityMesh VisibilityMesh { get; set; }

    public Scene Scene { get; set; }

    public void Draw(RenderTarget target, RenderStates states)
    {
        target.Draw(this.WallMesh, states);
        target.Draw(this.BlockMesh, states);
        target.Draw(this.PressureMesh, states);
        target.Draw(this.LightMesh, states);
        target.Draw(this.VisibilityMesh, states);
    }

    public void SlowUpdate()
    {
        this.WallMesh.SlowUpdate();
        this.BlockMesh.SlowUpdate();
        this.PressureMesh.SlowUpdate();
        this.LightMesh.SlowUpdate();
        this.VisibilityMesh.SlowUpdate();
    }

    public void FastUpdate()
    {
        this.WallMesh.FastUpdate();
        this.BlockMesh.FastUpdate();
        this.PressureMesh.FastUpdate();
        this.LightMesh.FastUpdate();
        this.VisibilityMesh.FastUpdate();
    }

    public void Dispose()
    {
        this.WallMesh.Dispose();
        this.BlockMesh.Dispose();
        this.PressureMesh.Dispose();
        this.LightMesh.Dispose();
        this.VisibilityMesh.Dispose();
    }

    public IEnumerator<(Block block, Wall wall)> GetEnumerator()
    {
        for (int x = 0; x < Chunk.Size.X; x++)
        {
            for (int y = 0; y < Chunk.Size.Y; y++)
            {
                yield return (this.BlockMesh.Grid[x, y], this.WallMesh.Grid[x, y]);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}
