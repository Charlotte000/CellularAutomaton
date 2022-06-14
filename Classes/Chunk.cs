namespace CellularAutomaton.Classes;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Mesh;
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
        // this.BlockMesh.Draw(target, states);
        foreach (var block in this.BlockMesh)
        {
            if (block.IsVisible)
            {
                var blockRenderState = new RenderStates(states);
                blockRenderState.Transform.Translate(block.CollisionBox.Position);
                target.Draw(block, blockRenderState);
            }
        }

        // this.PressureMesh.Draw(target, states);
    }

    public void Update(Scene scene)
    {
        foreach (var block in this.BlockMesh)
        {
            block.WasUpdated = false;
        }

        foreach (var block in this.BlockMesh)
        {
            if (!block.WasUpdated)
            {
                block.WasUpdated = true;
                block.OnUpdate(scene);
            }
        }

        this.PressureMesh.Update(scene);
    }

    public void Dispose()
    {
        foreach (var block in this.BlockMesh)
        {
            block.OnDelete();
        }
    }
}
