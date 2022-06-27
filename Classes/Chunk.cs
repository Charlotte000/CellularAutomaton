namespace CellularAutomaton.Classes;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Meshes;
using CellularAutomaton.Classes.Walls;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;
using System.Collections;

public class Chunk : IMonoBehaviour, Drawable, IEnumerable<(Block block, Wall wall)>
{
    private readonly List<IMesh> meshes;

    public Chunk(int x, int y)
    {
        this.Coord = new Vector2i(x, y);

        this.WallMesh = new (this, this.Coord);
        this.BlockMesh = new (this, this.Coord);
        this.PressureMesh = new (this, this.Coord);
        this.LightMesh = new (this, this.Coord);
        this.VisibilityMesh = new (this, this.Coord);

        this.meshes = new ()
        {
            this.WallMesh, this.BlockMesh, this.PressureMesh, this.LightMesh, this.VisibilityMesh,
        };
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
        foreach (var mesh in this.meshes)
        {
            target.Draw(mesh, states);
        }
    }

    public void OnCreate()
    {
    }

    public void OnFixedUpdate()
    {
        foreach (var mesh in this.meshes)
        {
            mesh.OnFixedUpdate();
        }
    }

    public void OnUpdate()
    {
        foreach (var mesh in this.meshes)
        {
            mesh.OnUpdate();
        }
    }

    public void OnDestroy()
    {
        foreach (var mesh in this.meshes)
        {
            mesh.OnDestroy();
        }
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
