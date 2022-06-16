namespace CellularAutomaton.Classes;

using CellularAutomaton.Classes.Meshes;
using SFML.Graphics;
using SFML.System;

public class Chunk : Drawable
{
    public Chunk(int x, int y)
    {
        this.Coord = new Vector2i(x, y);

        this.BlockMesh = new (this.Coord);
        this.PressureMesh = new (this.Coord);
    }

    public static Vector2i Size { get; } = new (20, 20);

    public Vector2i Coord { get; set; }

    public BlockMesh BlockMesh { get; set; }

    public PressureMesh PressureMesh { get; set; }

    public void Draw(RenderTarget target, RenderStates states)
    {
        // this.BlockMesh.DrawMesh(target);
        // this.PressureMesh.DrawMesh(target);
        target.Draw(this.BlockMesh, states);
        target.Draw(this.PressureMesh, states);
    }

    public void Update(Scene scene)
    {
        this.BlockMesh.Update(scene);
        this.PressureMesh.Update(scene);
    }

    public void Dispose()
    {
        this.BlockMesh.Dispose();
        this.PressureMesh.Dispose();
    }
}
