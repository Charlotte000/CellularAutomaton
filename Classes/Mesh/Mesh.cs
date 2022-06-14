namespace CellularAutomaton.Classes.Mesh;

using SFML.System;
using System.Collections;

public abstract class Mesh<T> : IEnumerable<T>
{
    public Mesh(Vector2i size, Vector2i coord)
    {
        this.Grid = new T[size.X, size.Y];
        this.Coord = coord;
    }

    public T[,] Grid { get; set; }

    public virtual Vector2i Coord { get; set; }

    public int Width { get => this.Grid.GetLength(0); }

    public int Height { get => this.Grid.GetLength(1); }

    public virtual T? this[int x, int y]
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

    public T? this[Index x, Index y]
    {
        get => this[x.GetOffset(this.Width), y.GetOffset(this.Height)];

        set => this[x.GetOffset(this.Width), y.GetOffset(this.Height)] = value;
    }

    public T? this[Vector2i coord]
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

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var chunk in this.Grid)
        {
            yield return chunk;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
        => this.Grid.GetEnumerator();
}
