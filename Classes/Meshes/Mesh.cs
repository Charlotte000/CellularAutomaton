namespace CellularAutomaton.Classes.Meshes;

using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;
using System.Collections;

public abstract class Mesh<TValue, TParent> : IMesh, IEnumerable<TValue>, Drawable
{
    public Mesh(TParent parent, Vector2i size, Vector2i coord)
    {
        this.Grid = new TValue[size.X, size.Y];
        this.Coord = coord;
        this.Parent = parent;
        this.OnCreate();
    }

    public Mesh(TParent parent, Vector2i size)
    {
        this.Grid = new TValue[size.X, size.Y];
        this.Parent = parent;
        this.OnCreate();
    }

    public TParent Parent { get; set; }

    public TValue[,] Grid { get; set; }

    public virtual Vector2i Coord { get; set; }

    public int Width { get => this.Grid.GetLength(0); }

    public int Height { get => this.Grid.GetLength(1); }

    public virtual TValue? this[int x, int y]
    {
        get
        {
            if (this.IsValidCoord(x, y))
            {
                return this.Grid[x - this.Coord.X, y - this.Coord.Y];
            }

            return default;
        }

        set
        {
            if (this.IsValidCoord(x, y))
            {
                this.Grid[x - this.Coord.X, y - this.Coord.Y] = value!;
            }
        }
    }

    public TValue? this[Index x, Index y]
    {
        get => this[x.GetOffset(this.Width), y.GetOffset(this.Height)];

        set => this[x.GetOffset(this.Width), y.GetOffset(this.Height)] = value;
    }

    public TValue? this[Vector2i coord]
    {
        get => this[coord.X, coord.Y];

        set => this[coord.X, coord.Y] = value;
    }

    public virtual bool IsValidCoord(int x, int y)
    {
        var localX = x - this.Coord.X;
        var localY = y - this.Coord.Y;
        return localX >= 0 && localX < this.Width && localY >= 0 && localY < this.Height;
    }

    public virtual void Draw(RenderTarget target, RenderStates states)
    {
    }

    public virtual void DrawMesh(RenderTarget target)
    {
    }

    public virtual void OnCreate()
    {
    }

    public virtual void OnUpdate()
    {
    }

    public virtual void OnFixedUpdate()
    {
    }

    public virtual void OnDestroy()
    {
    }

    public IEnumerator<TValue> GetEnumerator()
    {
        foreach (var chunk in this.Grid)
        {
            yield return chunk;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
        => this.Grid.GetEnumerator();
}