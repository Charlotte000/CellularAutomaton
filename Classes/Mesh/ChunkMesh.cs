namespace CellularAutomaton.Classes.Mesh;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Utils;
using SFML.System;

public class ChunkMesh : Mesh<Chunk>, IEnumerable<Block>
{
    public ChunkMesh()
        : base(new Vector2i(4, 4), new Vector2i(0, 0))
    {
        for (int x = 0; x < this.Width; x++)
        {
            for (int y = 0; y < this.Height; y++)
            {
                this.Grid[x, y] = new Chunk(x * Chunk.Size.X, y * Chunk.Size.Y);
            }
        }
    }

    public override Chunk? this[int x, int y]
    {
        get
        {
            if (this.IsValidCoord(x, y))
            {
                var localCoord = new Vector2i(x, y) - this.Grid[0, 0].Coord;
                return this.Grid[localCoord.X / Chunk.Size.X, localCoord.Y / Chunk.Size.Y];
            }

            return null;
        }
    }

    public override Vector2i Coord { get => this.Grid[0, 0].Coord; }

    public override bool IsValidCoord(int x, int y)
    {
        var topLeftCoord = this.Grid[0, 0].Coord;
        var bottomRightCoord = this.Grid[this.Width - 1, this.Height - 1].Coord + Chunk.Size;
        return x >= topLeftCoord.X && x < bottomRightCoord.X && y >= topLeftCoord.Y && y < bottomRightCoord.Y;
    }

    public bool Update(Scene scene)
    {
        var cameraCoord = scene.Entities[0].CollisionBox.Position / Block.Size;
        if (cameraCoord.X < this.Grid[1, 0].Coord.X)
        {
            ChunkMoveHelper.MoveChunksLeft(scene);
            return true;
        }

        if (cameraCoord.X > this.Grid[this.Width - 2, 0].Coord.X + Chunk.Size.X)
        {
            ChunkMoveHelper.MoveChunksRight(scene);
            return true;
        }

        if (cameraCoord.Y < this.Grid[0, 1].Coord.Y)
        {
            ChunkMoveHelper.MoveChunksUp(scene);
            return true;
        }

        if (cameraCoord.Y > this.Grid[0, this.Height - 2].Coord.Y + Chunk.Size.Y)
        {
            ChunkMoveHelper.MoveChunksDown(scene);
            return true;
        }

        return false;
    }

    IEnumerator<Block> IEnumerable<Block>.GetEnumerator()
    {
        foreach (var chunk in this.Grid)
        {
            foreach (var block in chunk.BlockMesh.Grid)
            {
                yield return block;
            }
        }
    }
}